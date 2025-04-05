using Rent.Application.Interfaces;
using Rent.Application.Interfaces.Rent.Application.Interfaces;
using Rent.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace Rent.Application.Services
{
    public class CarAvailabilityChecker : ICarAvailabilityChecker
    {
        private readonly IUnitOfWork _unitOfWork;

        public CarAvailabilityChecker(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsCarAvailable(Guid carId, DateTime startDate, DateTime endDate)
        {
            var reservations = await _unitOfWork.ReservationRepository.FindAsync(r =>
                r.CarId == carId && r.StartDate < endDate && r.EndDate > startDate);

            return !reservations.Any();
        }
    }
}
