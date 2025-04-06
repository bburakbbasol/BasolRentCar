
using Rent.Infrastructure.Entities;
using Rent.Infrastructure.Repository;

namespace Rent.Infrastructure.Data
{
    // UnitOfWork, tüm repository'leri yöneten ve veritabanı işlemleri için merkezi bir sınıf sağlar.
    public class UnitOfWork : IUnitOfWork
    {
        // AppDbContext, veritabanı bağlantısını ve veri işlemleri için kullanılan DbSet'leri içerir.
        private readonly AppDbContext _context;

        // Farklı veri tabanı varlıkları (Car, User, Reservation) için repository'ler
        private IGenericRepository<Car> _carRepository;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Reservation> _reservationRepository;

        // UnitOfWork sınıfının yapıcı metodu (constructor), AppDbContext nesnesini alır ve burada kullanır.
        public UnitOfWork(AppDbContext context)
        {
            _context = context; // Veritabanı bağlantısını başlatır.
        }

        // Araba (Car) repository'sini döndüren property.
        // Eğer _carRepository zaten oluşturulmuşsa, onu kullanır. Yoksa yeni bir GenericRepository<Car> oluşturur.
        public IGenericRepository<Car> CarRepository =>
            _carRepository ??= new GenericRepository<Car>(_context);

        // Kullanıcı (User) repository'sini döndüren property.
        // Eğer _userRepository zaten oluşturulmuşsa, onu kullanır. Yoksa yeni bir GenericRepository<User> oluşturur.
        public IGenericRepository<User> UserRepository =>
            _userRepository ??= new GenericRepository<User>(_context);

        // Rezervasyon (Reservation) repository'sini döndüren property.
        // Eğer _reservationRepository zaten oluşturulmuşsa, onu kullanır. Yoksa yeni bir GenericRepository<Reservation> oluşturur.
        public IGenericRepository<Reservation> ReservationRepository =>
            _reservationRepository ??= new GenericRepository<Reservation>(_context);

        // Veritabanındaki değişiklikleri kaydetmek için kullanılan metot.
        // SaveChangesAsync, asenkron olarak veritabanındaki değişiklikleri kaydeder.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); // Veritabanı işlemlerini kaydeder.
        }

        // UnitOfWork sınıfı ile ilişkili kaynakları serbest bırakmak için kullanılan metot.
        // Veritabanı bağlantısının düzgün bir şekilde kapanmasını sağlar.
        public void Dispose()
        {
            _context.Dispose(); // Veritabanı bağlantısını serbest bırakır.
        }
    }
}
