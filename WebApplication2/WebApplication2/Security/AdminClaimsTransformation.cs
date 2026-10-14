using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace WebApplication2.Security
{
    public sealed class AdminClaimsTransformation : IClaimsTransformation
    {
        private static readonly HashSet<string> AdminEmails = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin@shop.com",
            "admin@gmail.com"
        };

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated == true)
            {
                string? email = principal.FindFirstValue(ClaimTypes.Email) ?? principal.Identity.Name;
                if (email != null && AdminEmails.Contains(email) && !principal.IsInRole("Admin"))
                {
                    (principal.Identity as ClaimsIdentity)?.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                }
            }
            return Task.FromResult(principal);
        }
    }
}
