using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vitalis.Data.Entities;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vitalis.Core.Models.User;

namespace Vitalis.Controllers
{
    [ApiController]
    [Route("auth")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;

        public UserController(UserManager<User> _userManager, IConfiguration config)
        {
            userManager = _userManager;
            this.config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new User()
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name
            };

            var res = await userManager.CreateAsync(user, model.Password);
            if (res.Succeeded)
            {
                return Ok();
            }

            //foreach (var error in res.Errors)
            //{
            //    ModelState.AddModelError("ConfirmPassword", error.Description);
            //}

            return BadRequest(res.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(model);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user, model.RememberMe);
                response = Ok(new { token = tokenString, user = user });
            }

            return response;
        }

        private string GenerateJSONWebToken(UserModel userInfo, bool rememberMe)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id)
            };

            var tokenExpiry = rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(120);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Issuer"],
                claims,
                expires: tokenExpiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserModel> AuthenticateUser(LoginViewModel login)
        {
            var user = await userManager.FindByNameAsync(login.Email) ??
                       await userManager.FindByEmailAsync(login.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
            {
                return null;
            }

            return new UserModel
            {
                Name = user.UserName,
                Email = user.Email,
                Id = user.Id
            };
        }

        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user != null)
        //    {
        //        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        //        var link = Url.Action("ResetPassword", "User", new { token, email = user.Email }, Request.Scheme);
        //        var message = new EmailMessage(email, "Password reset", GetEmailTemplate(link));
        //        bool success = await emailService.SendAsync(message, new CancellationToken());
        //        if (success)
        //        {
        //            TempData[Constraints.Message] = EmailSentMsg;
        //        }
        //        else
        //        {
        //            TempData[Constraints.Message] = SomethingWrongMsg;
        //        }
        //    }

        //    return RedirectToAction("Login");
        //}
    }
}
