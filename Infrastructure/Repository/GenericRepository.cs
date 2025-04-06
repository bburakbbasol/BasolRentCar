
using Microsoft.EntityFrameworkCore;
using Rent.Infrastructure.Data;
using System.Linq.Expressions;

namespace Rent.Infrastructure.Repository
{
    // GenericRepository sınıfı, genel CRUD (Create, Read, Update, Delete) işlemleri için kullanılan bir repository'dir.
    // T parametresi, repository'nin hangi türdeki veri modeliyle çalışacağını belirtir.
    // T türü sınıf (class) türünden olmalıdır.
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Veritabanı bağlamını (DbContext) tutan değişken
        private readonly AppDbContext _context;

        // T türündeki verinin bulunduğu DbSet'i tutan değişken
        private readonly DbSet<T> _dbSet;

        // GenericRepository sınıfının yapıcı metodu (constructor), AppDbContext'i alır ve DbSet<T> oluşturur.
        public GenericRepository(AppDbContext context)
        {
            _context = context; // Veritabanı bağlamını alır.
            _dbSet = context.Set<T>(); // Veritabanı bağlamından DbSet<T> nesnesini oluşturur.
        }

        // Belirtilen id'ye sahip bir nesneyi asenkron olarak bulur.
        public async Task<T> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id); // DbSet'teki nesneleri id ile arar ve döner.

        // Tüm verileri asenkron olarak alır.
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync(); // DbSet'teki tüm nesneleri liste halinde döner.

        // Verilen koşula (expression) uyan tüm verileri asenkron olarak alır.
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
            => await _dbSet.Where(expression).ToListAsync(); // Koşula uyan verileri alır.

        // Yeni bir nesneyi veritabanına asenkron olarak ekler.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // Veritabanına ekler.
            await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydeder.
        }

        // Veritabanındaki mevcut bir nesneyi asenkron olarak günceller.
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity); // Veritabanındaki nesneyi günceller.
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder.
        }

        // Veritabanındaki bir nesneyi asenkron olarak siler.
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity); // Veritabanındaki nesneyi siler.
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder.
        }

        // Veritabanındaki tüm değişiklikleri asenkron olarak kaydeder.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); // Veritabanına tüm değişiklikleri kaydeder.
        }
    }
}
