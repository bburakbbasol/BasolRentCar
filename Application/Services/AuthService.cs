
using Rent.Application.DTOs;               
using Rent.Application;              
using Rent.Infrastructure.Entities;       
using Rent.Infrastructure.Repository;     
using System.Text;                        
using System.Security.Cryptography;   
using Rent.Application.Services;         

// IAuthService arayüzünü uygulayan AuthService sınıfı
public class AuthService : IAuthService
{
    // Kullanıcı verilerine erişim için generic repository
    private readonly IGenericRepository<User> _userRepository;

    // Constructor ile repository enjekte ediliyor
    public AuthService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    // Kullanıcı doğrulama (login gibi): kullanıcı adı ve şifre kontrolü
    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        // Kullanıcıyı kullanıcı adına göre arar
        var user = (await _userRepository.FindAsync(u => u.Username == username)).FirstOrDefault();
        if (user == null) return false; // Kullanıcı yoksa başarısız

        // Şifreyi hash'leyip kayıtlı hash ile karşılaştırır
        var hashedPassword = HashPassword(password, user.Salt);
        return hashedPassword == user.PasswordHash;
    }

    // Yeni kullanıcı kaydı (normal kullanıcı)
    public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
    {
        // Aynı kullanıcı adı ya da e-posta ile kullanıcı varsa kayıt yapılmaz
        var existingUser = (await _userRepository.FindAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email)).FirstOrDefault();
        if (existingUser != null) return false;

        // Yeni bir salt oluşturulur (şifreyi hashlemek için kullanılır)
        var salt = Guid.NewGuid().ToString();

        // Yeni kullanıcı nesnesi oluşturulur
        var user = new User
        {
            Id = Guid.NewGuid(),                         // Yeni benzersiz ID
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password, salt),  // Şifre hash'lenmiş haliyle saklanır
            Salt = salt,
            Role = "User",                               // Rol varsayılan olarak "User"
            CreatedAt = DateTime.UtcNow,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        // Veritabanına eklenir
        await _userRepository.AddAsync(user);
        return true;
    }

    // Yeni admin kullanıcısı kaydı (kullanıcıdan farklı olarak rol "Admin")
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
            Role = "Admin",                              // Burada rol "Admin"
            CreatedAt = DateTime.UtcNow,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        await _userRepository.AddAsync(user);
        return true;
    }

    // Kullanıcıyı ID'ye göre getirir
    public async Task<User> GetById(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    // Kullanıcıyı kullanıcı adına göre getirir
    public async Task<User> GetByUsername(string username)
    {
        return (await _userRepository.FindAsync(u => u.Username == username)).FirstOrDefault();
    }

    // Kullanıcı girişi yapılır ve sonucu (başarılı/başarısız) döner
    public async Task<Result<User>> LoginUser(LoginUserDto loginUserDto)
    {
        // E-posta ile kullanıcı aranır
        var user = (await _userRepository.FindAsync(u => u.Email == loginUserDto.Email)).FirstOrDefault();
        if (user == null)
            return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        // Şifre hash'lenerek karşılaştırılır
        var hashedPassword = HashPassword(loginUserDto.Password, user.Salt);
        if (hashedPassword != user.PasswordHash)
            return new Result<User> { IsSucced = false, Message = "Invalid email or password" };

        // Giriş başarılıysa kullanıcı bilgileri döner
        return new Result<User> { IsSucced = true, Data = user };
    }

    // Şifreyi SHA256 algoritmasıyla hash'ler
    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create()) // SHA256 nesnesi oluşturuluyor
        {
            // Şifre + salt birleştirilip UTF8 ile byte dizisine dönüştürülür
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            // Byte dizisi Base64 string'e çevrilir
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
