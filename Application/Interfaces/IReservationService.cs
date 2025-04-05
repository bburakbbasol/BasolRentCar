using Rent.Application.DTOs;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId);
        Task<Reservation> CreateReservationAsync(string userId, ReservationCreateDto dto);
        Task<Reservation> GetReservationByIdAsync(Guid id);
        Task UpdateReservationAsync(Reservation reservation);
        Task CancelReservationAsync(Guid id);
    }
}
