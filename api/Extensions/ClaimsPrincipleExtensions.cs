using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsserName(this ClaimsPrincipal user) 
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUsserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}
