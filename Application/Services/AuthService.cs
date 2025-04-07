using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Rent.Application.DTOs;
using Rent.Application;
using Rent.Infrastructure.Entities;
using Rent.Infrastructure.Repository;
using Rent.WebApi.Jwt;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Rent.Application.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IDataProtector _dataProtector;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;

    public AuthService(IGenericRepository<User> userRepository, IDataProtectionProvider dataProtectionProvider, JwtHelper jwtHelper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _dataProtector = dataProtectionProvider.CreateProtector("AuthService");
        _jwtHelper = jwtHelper;
        _configuration = configuration;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = (await _userRepository.FindAsync(u => u.Username == username)).FirstOrDefault();
        if (user == null) return false;

        var encryptedPassword = Encrypt(password);
        return encryptedPassword == user.PasswordHash;
    }

    public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
    {
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null) return false;

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
        return true;
    }

    public async Task<bool> RegisterAdminAsync(RegisterDto registerDto)
    {
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null) return false;

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
        return true;
    }

    public async Task<bool> RegisterAdminAsyncIData(RegisterDto registerDto)
    {
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null) return false;

        var salt = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = Encrypt(registerDto.Password), // Şifreyi şifrele
            Salt = salt,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        await _userRepository.AddAsync(user);
        return true;
    }

    public async Task<Result<User>> LoginAdmin(LoginUserDto loginUserDto)
    {
        var user = (await _userRepository.FindAsync(u => u.Email == loginUserDto.Email && u.Role == "Admin")).FirstOrDefault();
        if (user == null) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var hashedPassword = HashPassword(loginUserDto.Password, user.Salt);
        if (hashedPassword != user.PasswordHash) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var jwtDto = new JwtDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            UserName = user.Username,
            SecretKey = _configuration["Jwt:SecretKey"],
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            ExpiresMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"])
        };

        var token = _jwtHelper.GenerateJwtToken(jwtDto);

        return new Result<User> { IsSucced = true, Data = user, Token = token };
    }

    public async Task<Result<User>> LoginUser(LoginUserDto loginUserDto)
    {
        var user = (await _userRepository.FindAsync(u => u.Email == loginUserDto.Email)).FirstOrDefault();
        if (user == null) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var hashedPassword = HashPassword(loginUserDto.Password, user.Salt);
        if (hashedPassword != user.PasswordHash) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var jwtDto = new JwtDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            UserName = user.Username,
            SecretKey = _configuration["Jwt:SecretKey"],
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            ExpiresMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"])
        };

        var token = _jwtHelper.GenerateJwtToken(jwtDto);

        return new Result<User> { IsSucced = true, Data = user, Token = token };
    }

    public async Task<Result<User>> LoginIData(LoginUserDto loginUserDto)
    {
        var user = (await _userRepository.FindAsync(u => u.Email == loginUserDto.Email)).FirstOrDefault();
        if (user == null) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var decryptedPassword = Decrypt(user.PasswordHash); // Şifreyi çöz
        if (loginUserDto.Password != decryptedPassword) return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        var jwtDto = new JwtDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            UserName = user.Username,
            SecretKey = _configuration["Jwt:SecretKey"],
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            ExpiresMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"])
        };

        var token = _jwtHelper.GenerateJwtToken(jwtDto);

        return new Result<User> { IsSucced = true, Data = user, Token = token };
    }

    public async Task<User> GetById(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> GetByUsername(string username)
    {
        return (await _userRepository.FindAsync(u => u.Username == username)).FirstOrDefault();
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private string Encrypt(string input)
    {
        return _dataProtector.Protect(input);
    }

    private string Decrypt(string input)
    {
        return _dataProtector.Unprotect(input);
    }
}
