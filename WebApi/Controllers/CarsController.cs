using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Application.DTOs;
using Rent.Application.Services;
using Rent.Infrastructure.Entities;
using Rent.WebApi.Filters;
using Rent.WebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    // Araç işlemlerini yapmak için servis sınıfımızı enjekte ediyoruz.
    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    // Sistemdeki tüm araçları getiriyoruz.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        var cars = await _carService.GetAllCarsAsync();
        return Ok(cars);
    }

    // Belirli bir plaka numarasına göre araç getiriyoruz.
    [HttpGet("{plateNumber}")]
    [ServiceFilter(typeof(TimeControllerFilter))] // Zaman kontrolü için filtre kullanıyoruz.
    public async Task<ActionResult<Car>> GetCar(string plateNumber)
    {
        var car = await _carService.GetCarByPlateNumberAsync(plateNumber);
        if (car == null)
            return NotFound(); // Araç bulunamazsa 404 dönüyoruz.
        return Ok(car);
    }

    // Yeni bir araç ekliyoruz. Sadece admin yetkisine sahip kullanıcılar erişebilir.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CarDto>> CreateCar(CarModel carModel)
    {
        // Modelden DTO oluşturuyoruz.
        var carDto = new CarDto
        {
            Brand = carModel.Brand,
            Model = carModel.Model,
            Year = carModel.Year,
            PlateNumber = carModel.PlateNumber,
            DailyRate = carModel.DailyRate
        };

        // Aracı kaydetmek için servisi kullanıyoruz.
        var result = await _carService.CreateCarAsync(carDto);
        if (!result.IsSucced)
        {
            return BadRequest(result.Message);
        }
        else
        {
            // Başarılı olursa 201 ile birlikte lokasyon bilgisiyle dönüyoruz.
            return CreatedAtAction(nameof(GetCar), new { plateNumber = result.Data.PlateNumber }, result.Data);
        }
    }

    // Bir aracın tüm bilgilerini güncelliyoruz.
    [HttpPut("{plateNumber}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(TimeControllerFilter))]
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

        // Güncelleme işlemini yapıyoruz.
        await _carService.UpdateCarAsync(plateNumber, updateCarDto);
        return NoContent(); // Başarılıysa içerik dönmüyoruz.
    }

    // Bir aracın sadece bazı bilgilerini (örneğin müsaitlik/dailyRate) güncelliyoruz.
    [HttpPatch("{plateNumber}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchCar(string plateNumber, [FromBody] PatchCarRequest patchCarRequest)
    {
        var patchCarDto = new PatchCarDto
        {
            IsAvailable = patchCarRequest.IsAvailable,
            DailyRate = patchCarRequest.DailyRate
        };

        await _carService.PatchCarAsync(plateNumber, patchCarDto);
        return NoContent();
    }

    // Güncelleme işlemi için gerekli verileri alıyoruz (formları doldurmak için).
    [HttpGet("update/{plateNumber}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UpdateCarRequest>> GetCarForUpdate(string plateNumber)
    {
        var car = await _carService.GetCarByPlateNumberAsync(plateNumber);
        if (car == null)
            return NotFound();

        // Mevcut bilgileri DTO formatında döndürüyoruz.
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

    // Aracı silmek yerine soft-delete yapıyoruz.
    [HttpDelete("{plateNumber}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCar(string plateNumber)
    {
        await _carService.DeleteCarAsync(plateNumber);
        return NoContent();
    }
}
