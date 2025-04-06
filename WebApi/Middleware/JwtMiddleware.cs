// Gerekli namespace'ler ekleniyor
using Microsoft.IdentityModel.Tokens;
using Rent.Application.Services;
using Rent.WebApi.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// JWT doğrulaması için özel middleware sınıfı
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;
    private readonly IConfiguration _configuration;

    // Middleware için constructor: gerekli bağımlılıklar enjekte ediliyor
    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    // Middleware’in çalıştığı kısım, her HTTP isteği için tetiklenir
    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        // Authorization başlığından token'ı al (Bearer token şeklindedir)
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        // Token varsa kullanıcıyı doğrulamak için işlemlere başla
        if (token != null)
        {
            _logger.LogInformation("Token bulundu: " + token);
            attachUserToContext(context, authService, token);
        }
        else
        {
            _logger.LogInformation("Token bulunamadı.");
        }

        // İşlem bittikten sonra diğer middleware'lere devam et
        await _next(context);
    }

    // Kullanıcıyı bağlam (context) içine ekleyen yardımcı metot
    private void attachUserToContext(HttpContext context, IAuthService authService, string token)
    {
        try
        {
            // JWT token çözücü oluşturuluyor
            var tokenHandler = new JwtSecurityTokenHandler();

            // appsettings.json dosyasındaki gizli anahtar okunuyor
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            _logger.LogInformation("Token doğrulama işlemi başlatıldı.");

            // Token’ın geçerliliği kontrol ediliyor (issuer, audience, imza, süre vs.)
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // İmzanın doğruluğu kontrol edilecek
                IssuerSigningKey = new SymmetricSecurityKey(key), // İmza anahtarı

                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"], // Doğru issuer olmalı

                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"], // Doğru audience olmalı

                ClockSkew = TimeSpan.Zero // Zaman farkı toleransı yok
            }, out SecurityToken validatedToken);

            // Token doğrulandıktan sonra içinden kullanıcı adı çekiliyor
            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(x => x.Type == JwtClaimNames.Username).Value;

            // Kullanıcı adı ile veritabanından kullanıcı bilgisi alınıp context'e ekleniyor
            context.Items["User"] = authService.GetByUsername(username).Result;

            _logger.LogInformation("Token doğrulama işlemi başarılı. Kullanıcı adı: " + username);
        }
        catch (Exception ex)
        {
            // Token geçersizse hata loglanır ama istek engellenmez
            _logger.LogError(ex, "Token validation failed");
        }
    }
}
