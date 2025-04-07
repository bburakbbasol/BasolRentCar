using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rent.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rent.WebApi.Jwt
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(JwtDto jwtDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtDto.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtClaimNames.Id, jwtDto.Id.ToString()),
                    new Claim(JwtClaimNames.Email, jwtDto.Email),
                    new Claim(JwtClaimNames.FirstName, jwtDto.FirstName),
                    new Claim(JwtClaimNames.LastName, jwtDto.LastName),
                    new Claim(JwtClaimNames.Role, jwtDto.Role),
                    new Claim(JwtClaimNames.Username, jwtDto.UserName) 
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtDto.ExpiresMinutes),
                Issuer = jwtDto.Issuer,
                Audience = jwtDto.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}


