using Rent.Application.DTOs;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByPlateNumberAsync(string plateNumber);
        Task<Result<CarDto>> CreateCarAsync(CarDto car);
        Task UpdateCarAsync(string plateNumber, UpdateCarDto carDto);
        Task DeleteCarAsync(string plateNumber);
        Task<IEnumerable<Car>> GetAvailableCarsAsync();
    }
}

