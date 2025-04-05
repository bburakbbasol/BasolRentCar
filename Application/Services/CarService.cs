using Rent.Application.DTOs;
using Rent.Infrastructure.Data;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _unitOfWork.CarRepository.FindAsync(c => !c.IsDeleted);
        }

        public async Task<Car> GetCarByPlateNumberAsync(string plateNumber)
        {
            return (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber && !c.IsDeleted)).FirstOrDefault();
        }

        public async Task<Result<CarDto>> CreateCarAsync(CarDto carDto)
        {
            var car = new Car
            {
                Id = Guid.NewGuid(),
                Brand = carDto.Brand,
                Model = carDto.Model,
                Year = carDto.Year,
                PlateNumber = carDto.PlateNumber,
                DailyRate = carDto.DailyRate,
                IsAvailable = true,
                IsDeleted = false
            };

            await _unitOfWork.CarRepository.AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            carDto.Id = car.Id;
            carDto.IsAvailable = car.IsAvailable;

            return new Result<CarDto>
            {
                IsSucced = true,
                Message = "Car created successfully",
                Data = carDto
            };
        }

        public async Task UpdateCarAsync(string plateNumber, UpdateCarDto carDto)
        {
            var car = (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber && !c.IsDeleted)).FirstOrDefault();
            if (car == null)
            {
                throw new Exception("Car not found or has been deleted.");
            }

            car.Brand = carDto.Brand;
            car.Model = carDto.Model;
            car.Year = carDto.Year;
            car.DailyRate = carDto.DailyRate;
            car.IsAvailable = carDto.IsAvailable;

            await _unitOfWork.CarRepository.UpdateAsync(car);
        }

        public async Task DeleteCarAsync(string plateNumber)
        {
            var car = (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber)).FirstOrDefault();
            if (car != null)
            {
                car.IsDeleted = true;
                await _unitOfWork.CarRepository.UpdateAsync(car);
            }
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            return await _unitOfWork.CarRepository.FindAsync(c => c.IsAvailable && !c.IsDeleted);
        }
    }
}

