using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationR.Models;

namespace WebApplicationR.Security.Jwt
{
    public class JwtGenerator:IJwtGenerator
    {
        public string GenerateToken(User user)
        {
            var claim = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("here you must write the hash for encrypt token /////////"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var TokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = credentials

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(TokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}

