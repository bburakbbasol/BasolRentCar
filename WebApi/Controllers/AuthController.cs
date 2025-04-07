using Microsoft.AspNetCore.Mvc;
using Rent.Application.DTOs;
using Rent.Application.Services;
using Rent.WebApi.Jwt;
using Rent.WebApi.Models;
using System.Threading.Tasks;

namespace Rent.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IAuthService authService, JwtHelper jwtHelper)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginUserDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _authService.LoginUser(loginUserDto);

            if (!result.IsSucced)
                return BadRequest(result.Message);

            var user = result.Data;
            var jwtDto = new JwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                UserName = user.Username,
                SecretKey = _jwtHelper.SecretKey,
                Issuer = _jwtHelper.Issuer,
                Audience = _jwtHelper.Audience,
                ExpiresMinutes = _jwtHelper.ExpiresMinutes
            };

            var token = _jwtHelper.GenerateJwtToken(jwtDto);

            return Ok(new LoginResponse
            {
                Message = "Login successfully completed",
                Token = token
            });
        }

        [HttpPost("login-iData")]
        public async Task<IActionResult> LoginIData([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginUserDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _authService.LoginIData(loginUserDto);

            if (!result.IsSucced)
                return BadRequest(result.Message);

            var user = result.Data;
            var jwtDto = new JwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                UserName = user.Username,
                SecretKey = _jwtHelper.SecretKey,
                Issuer = _jwtHelper.Issuer,
                Audience = _jwtHelper.Audience,
                ExpiresMinutes = _jwtHelper.ExpiresMinutes
            };

            var token = _jwtHelper.GenerateJwtToken(jwtDto);

            return Ok(new LoginResponse
            {
                Message = "Login successfully completed",
                Token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var registerDto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _authService.RegisterUserAsync(registerDto);

            if (!result)
            {
                return BadRequest("User already exists.");
            }

            return Ok("User registered successfully.");
        }

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

        [HttpPost("register-admin-iData")]
        public async Task<IActionResult> RegisterAdminAsyncIData([FromBody] RegisterModel model)
        {
            var registerDto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _authService.RegisterAdminAsyncIData(registerDto);
            if (!result)
            {
                return BadRequest("Admin already exists.");
            }

            return Ok("Admin registered successfully with IData.");
        }
    }
}





