using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TODO.Models;
using TODO.ViewModels;
using Microsoft.IdentityModel.Tokens;
namespace TODO.Services
{
    public class AuthenticationService:Service
    {
        
        public AuthenticationService(TodoContext db) : base(db) { }
        public string CreateToken(User user)
        {
            var identity = GetIdentity(user);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.AuthOptions.ISSUER,
                audience: AuthOptions.AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.AuthOptions.GetSymmetricSecurityKey()
                ,SecurityAlgorithms.HmacSha256)


                ) ;
            var encodedjwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedjwt;

        }

        private ClaimsIdentity GetIdentity(User user)
        {
            
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,user.UserName)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
    }
}
