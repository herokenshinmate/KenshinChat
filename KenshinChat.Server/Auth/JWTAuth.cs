using KenshinChat.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Server.Auth
{
    public class JWTAuth
    {
        private static string secret = "mX3iFmMaC1OSEzpeHZFPTwQs2stwa2BG";
        private static string issuer = "http://localhost:47252";

        public static string GenerateToken(User user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a list of claims
            List<Claim> Claims = new List<Claim>();
            Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            //Claims.Add(new Claim("user_id", user.Id.ToString()));
            Claims.Add(new Claim("username", user.Username));
            Claims.Add(new Claim("valid", "1"));

            //Create a Security Token object
            JwtSecurityToken token = new JwtSecurityToken(
                "SignalRTestServer", //Issuer
                "SignalRTests", //Audience
                Claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
