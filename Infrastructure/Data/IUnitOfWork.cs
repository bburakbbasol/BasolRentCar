using Rent.Infrastructure.Entities;
using Rent.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Car> CarRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Reservation> ReservationRepository { get; }
        Task SaveChangesAsync();
    }

}
