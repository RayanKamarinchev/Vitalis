using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vitalis.Core.Contracts;

namespace Vitalis.Controllers
{
    [ApiController]
    [Route("mol")]
    public class MoleculeController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly IMoleculeService moleculeService;

        public MoleculeController(IMoleculeService _moleculeService)
        {
            moleculeService = _moleculeService;
        }

        [HttpPost("getSmiles")]
        public string GetSmiles([FromBody] SingleValueJson input)
        {
            return moleculeService.ConvertFile(input.mol);
        }
    }

    public class SingleValueJson
    {
        public string mol { get; set; }
    }
}
