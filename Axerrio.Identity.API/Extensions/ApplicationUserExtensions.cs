using Axerrio.Identity.Accounts.Models;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Axerrio.Identity.API.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static ClaimsPrincipal ClaimsPrincipal(this ApplicationUser user)
        {
            var claims = user.GetClaimsFromUser();
            var identity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(identity);
        }

        public static IEnumerable<Claim> GetClaimsFromUser(this ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.LastName))
                claims.Add(new Claim("last_name", user.LastName));

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                claims.Add(new Claim("first_name", user.FirstName));


            claims.AddRange(new[]
            {
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            });

            return claims;
        }
    }
}
