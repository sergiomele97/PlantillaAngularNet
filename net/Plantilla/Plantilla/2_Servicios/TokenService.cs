using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Plantilla._2_Servicios
{
    public class TokenService
    {
        private readonly string _secret;
        private readonly int _expirationInMinutes;

        public TokenService(string secret, int expirationInMinutes)
        {
            _secret = secret;
            _expirationInMinutes = expirationInMinutes;
        }

        public string GenerateToken(string email)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                // Puedes agregar más claims según lo necesites
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(_expirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: null, // Puedes establecer un emisor si lo deseas
                audience: null, // Puedes establecer un público si lo deseas
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
