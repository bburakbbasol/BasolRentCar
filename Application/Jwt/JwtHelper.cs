using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rent.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rent.WebApi.Jwt
{
    // JWT token üretiminden sorumlu yardımcı sınıf
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        // appsettings.json dosyasındaki JWT ayarlarına erişmek için IConfiguration kullanılıyor
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Kullanıcıya ait bilgileri içeren JWT token üretir
        public string GenerateJwtToken(User user)
        {
            // Token işlemlerini yöneten handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // appsettings.json içinden SecretKey alınıp byte dizisine çevriliyor
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            // Token’a eklenecek kullanıcı bilgileri (claim'ler) tanımlanıyor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtClaimNames.Id, user.Id.ToString()),
                    new Claim(JwtClaimNames.Email, user.Email),
                    new Claim(JwtClaimNames.FirstName, user.FirstName),
                    new Claim(JwtClaimNames.LastName, user.LastName),
                    new Claim(JwtClaimNames.Role, user.Role),
                    new Claim(JwtClaimNames.Username, user.Username) // Kullanıcı adı ekleniyor
                }),

                // Token’ın geçerlilik süresi belirleniyor 
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"])),

                // Issuer ve Audience doğrulaması için ayarlar çekiliyor
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],

                // İmza algoritması ve gizli anahtar belirleniyor
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Token oluşturuluyor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Oluşturulan token string formatına dönüştürülüp geri döndürülüyor
            return tokenHandler.WriteToken(token);
        }
    }
}
