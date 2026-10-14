using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace WebApplication2.Security
{
    /// <summary>
    /// Maintient le rôle Admin pour les comptes administrateurs de démonstration,
    /// y compris lorsqu'un ancien cookie d'authentification est encore actif.
    /// </summary>
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
                string? email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.Identity.Name;

                if (email != null && AdminEmails.Contains(email) && !principal.IsInRole("Admin"))
                {
                    ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;
                    identity?.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                }
            }

            return Task.FromResult(principal);
        }
    }
}
