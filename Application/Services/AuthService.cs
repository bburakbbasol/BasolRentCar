using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rent.Application.DTOs;
using Rent.Application.Services;
using Rent.Application;
using Rent.Infrastructure.Entities;
using Rent.Infrastructure.Repository;
using Rent.WebApi.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IConfiguration _configuration;
    private readonly JwtHelper _jwtHelper;
    private readonly IDataProtector _dataProtector;

    public AuthService(IGenericRepository<User> userRepository, IConfiguration configuration, JwtHelper jwtHelper, IDataProtectionProvider dataProtectionProvider)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _jwtHelper = jwtHelper;
        _dataProtector = dataProtectionProvider.CreateProtector("AuthService");
    }

    public async Task<Result<User>> LoginUser(LoginUserDto loginUserDto)
    {
        var user = (await _userRepository.FindAsync(u => u.Email == loginUserDto.Email)).FirstOrDefault();
        if (user == null || !await ValidateUserAsync(user.Username, loginUserDto.Password))
        {
            return new Result<User>
            {
                IsSucced = false,
                Message = "Invalid email or password"
            };
        }

        return new Result<User>
        {
            IsSucced = true,
            Data = user
        };
    }

    public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
    {
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null)
        {
            return false; // Kullanıcı zaten mevcut
        }

        var salt = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password, salt),
            Salt = salt,
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RegisterAdminAsync(RegisterDto registerDto)
    {
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null)
        {
            return false; // Kullanıcı zaten mevcut
        }

        var salt = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password, salt),
            Salt = salt,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = (await _userRepository.FindAsync(u => u.Username == username)).FirstOrDefault();
        if (user == null) return false;

        var hashedPassword = HashPassword(password, user.Salt);
        return hashedPassword == user.PasswordHash;
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public async Task<User> GetById(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}
