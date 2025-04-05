using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Application.DTOs;
using Rent.Application.Services;
using Rent.Infrastructure.Entities;
using Rent.WebApi.Models;

namespace Rent.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        [HttpGet("{plateNumber}")]
        public async Task<ActionResult<Car>> GetCar(string plateNumber)
        {
            var car = await _carService.GetCarByPlateNumberAsync(plateNumber);
            if (car == null)
                return NotFound();
            return Ok(car);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CarDto>> CreateCar(CarModel carModel)
        {
            var carDto = new CarDto
            {
                Brand = carModel.Brand,
                Model = carModel.Model,
                Year = carModel.Year,
                PlateNumber = carModel.PlateNumber,
                DailyRate = carModel.DailyRate
            };

            var result = await _carService.CreateCarAsync(carDto);
            if (!result.IsSucced)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return CreatedAtAction(nameof(GetCar), new { plateNumber = result.Data.PlateNumber }, result.Data);
            }
        }

        [HttpPut("{plateNumber}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCar(string plateNumber, [FromBody] UpdateCarRequest updateCarRequest)
        {
            var car = await _carService.GetCarByPlateNumberAsync(plateNumber);
            if (car == null)
                return NotFound();

            var updateCarDto = new UpdateCarDto
            {
                Brand = updateCarRequest.Brand,
                Model = updateCarRequest.Model,
                Year = updateCarRequest.Year,
                DailyRate = updateCarRequest.DailyRate,
                IsAvailable = updateCarRequest.IsAvailable
            };

            await _carService.UpdateCarAsync(plateNumber, updateCarDto);
            return NoContent();
        }

        [HttpGet("update/{plateNumber}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateCarRequest>> GetCarForUpdate(string plateNumber)
        {
            var car = await _carService.GetCarByPlateNumberAsync(plateNumber);
            if (car == null)
                return NotFound();

            var updateCarRequest = new UpdateCarRequest
            {
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                DailyRate = car.DailyRate,
                IsAvailable = car.IsAvailable
            };

            return Ok(updateCarRequest);
        }

        [HttpDelete("{plateNumber}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCar(string plateNumber)
        {
            await _carService.DeleteCarAsync(plateNumber);
            return NoContent();
        }
    }
}
