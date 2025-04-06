
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Rent.Application.DTOs;           
using Rent.Application.Interfaces;    
using Rent.Application.Services;
using Rent.WebApi.Filters;            
using Rent.WebApi.Jwt;                
using Rent.WebApi.Models;             
using System.Threading.Tasks;

namespace Rent.WebApi.Controllers
{
    // Bu sınıf bir API Controller olduğunu belirtmek için işaretliyoruz.
    [ApiController]
    [Route("api/[controller]")] // Yani bu controller’a api/auth üzerinden erişeceğiz.
    public class AuthController : ControllerBase
    {
        // Kimlik doğrulama işlemleri için AuthService’i ve JWT üretmek için JwtHelper’ı kullanıyoruz.
        private readonly IAuthService _authService;
        private readonly JwtHelper _jwtHelper;

        // Constructor üzerinden bu servisleri alıyoruz.
        public AuthController(IAuthService authService, JwtHelper jwtHelper)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
        }

        // Kullanıcı giriş yaparken bu metodu kullanıyoruz.
        [HttpPost("login")]
       
        public async Task<IActionResult> Login(Models.LoginRequest request)
        {
            // Eğer gelen veri kurallara uygun değilse bad request dönüyoruz.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Email ve şifre ile kullanıcı giriş denemesi yapıyoruz.
            var result = await _authService.LoginUser(new LoginUserDto { Email = request.Email, Password = request.Password });

            // Giriş başarısızsa kullanıcıya mesaj dönüyoruz.
            if (!result.IsSucced)
                return BadRequest(result.Message);

            // Giriş başarılıysa kullanıcıdan JWT token üretiyoruz.
            var user = result.Data;
            var token = _jwtHelper.GenerateJwtToken(user);

            // Token ve başarılı mesajı ile geri dönüyoruz.
            return Ok(new LoginResponse
            {
                Message = "Login successfully completed",
                Token = token
            });
        }

        // Normal kullanıcı kaydı yapmak için bu endpoint’i kullanıyoruz.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Kullanıcıdan gelen modeli DTO'ya çeviriyoruz.
            var registerDto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // Kullanıcıyı kayıt etmeye çalışıyoruz.
            var result = await _authService.RegisterUserAsync(registerDto);

            // Kayıt başarısızsa uyarı dönüyoruz.
            if (!result)
            {
                return BadRequest("User already exists.");
            }

            // Başarılıysa kullanıcıya bilgi dönüyoruz.
            return Ok("User registered successfully.");
        }

        // Admin kullanıcı kaydı için ayrı bir endpoint yapıyoruz.
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var registerDto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _authService.RegisterAdminAsync(registerDto);
            if (!result)
            {
                return BadRequest("Admin already exists.");
            }

            return Ok("Admin registered successfully.");
        }
    }
}
