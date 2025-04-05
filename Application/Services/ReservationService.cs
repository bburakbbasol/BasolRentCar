//using Rent.Application.DTOs;
//using Rent.Application.Exceptions;
//using Rent.Application.Interfaces;
//using Rent.Application.Interfaces.Rent.Application.Interfaces;
//using Rent.Infrastructure.Data;
//using Rent.Infrastructure.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Rent.Application.Services
//{
//    public class ReservationService : IReservationService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ICarAvailabilityChecker _availabilityChecker;

//        public ReservationService(IUnitOfWork unitOfWork, ICarAvailabilityChecker availabilityChecker)
//        {
//            _unitOfWork = unitOfWork;
//            _availabilityChecker = availabilityChecker;
//        }

//        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId)
//        {
//            return await _unitOfWork.ReservationRepository.FindAsync(r => r.UserId == Guid.Parse(userId));
//        }

//        public async Task<Reservation> CreateReservationAsync(string userId, ReservationCreateDto dto)
//        {
//            if (!await _availabilityChecker.IsCarAvailable(dto.CarId, dto.StartDate, dto.EndDate))
//                throw new CarNotAvailableException();

//            var reservation = new Reservation
//            {
//                Id = Guid.NewGuid(),
//                UserId = Guid.Parse(userId),
//                CarId = dto.CarId,
//                StartDate = dto.StartDate,
//                EndDate = dto.EndDate,
//                Status = ReservationStatus.Active
//            };

//            await _unitOfWork.ReservationRepository.AddAsync(reservation);
//            await _unitOfWork.SaveChangesAsync();

//            return reservation;
//        }

//        public async Task<Reservation> GetReservationByIdAsync(Guid id)
//        {
//            return await _unitOfWork.ReservationRepository.GetByIdAsync(id);
//        }

//        public async Task UpdateReservationAsync(Reservation reservation)
//        {
//            await _unitOfWork.ReservationRepository.UpdateAsync(reservation);
//        }

//        public async Task CancelReservationAsync(Guid id)
//        {
//            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
//            if (reservation != null)
//            {
//                reservation.Status = ReservationStatus.Cancelled;
//                await _unitOfWork.ReservationRepository.UpdateAsync(reservation);
//            }
//        }
//    }
//}
using Rent.Application.DTOs;
using Rent.Application.Exceptions;
using Rent.Application.Interfaces;
using Rent.Application.Interfaces.Rent.Application.Interfaces;
using Rent.Infrastructure.Data;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rent.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICarAvailabilityChecker _availabilityChecker;

        public ReservationService(IUnitOfWork unitOfWork, ICarAvailabilityChecker availabilityChecker)
        {
            _unitOfWork = unitOfWork;
            _availabilityChecker = availabilityChecker;
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId)
        {
            return await _unitOfWork.ReservationRepository.FindAsync(r => r.UserId == Guid.Parse(userId));
        }

        public async Task<Reservation> CreateReservationAsync(string userId, ReservationCreateDto dto)
        {
            if (!await _availabilityChecker.IsCarAvailable(dto.CarId, dto.StartDate, dto.EndDate))
                throw new CarNotAvailableException();

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                CarId = dto.CarId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active"
            };

            await _unitOfWork.ReservationRepository.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            return reservation;
        }

        public async Task<Reservation> GetReservationByIdAsync(Guid id)
        {
            return await _unitOfWork.ReservationRepository.GetByIdAsync(id);
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            await _unitOfWork.ReservationRepository.UpdateAsync(reservation);
        }

        public async Task CancelReservationAsync(Guid id)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                await _unitOfWork.ReservationRepository.UpdateAsync(reservation);
            }
        }
    }
}

