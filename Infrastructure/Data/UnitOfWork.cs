using Microsoft.EntityFrameworkCore;

using Rent.Infrastructure.Entities;
using Rent.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IGenericRepository<Car> _carRepository;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Reservation> _reservationRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Car> CarRepository =>
            _carRepository ??= new GenericRepository<Car>(_context);

        public IGenericRepository<User> UserRepository =>
            _userRepository ??= new GenericRepository<User>(_context);

        public IGenericRepository<Reservation> ReservationRepository =>
            _reservationRepository ??= new GenericRepository<Reservation>(_context);


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
