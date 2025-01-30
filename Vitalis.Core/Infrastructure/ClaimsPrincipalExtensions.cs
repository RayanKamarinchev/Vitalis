using System.Security.Claims;
using Vitalis.Data.Entities;

namespace Vitalis.Core.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "Id").Value;
        }
    }
}
