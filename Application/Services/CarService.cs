// Gerekli namespace'leri kullanıyoruz, DTO'lar, veritabanı erişimi, entity'ler vb.
using Rent.Application.DTOs;
using Rent.Infrastructure.Data;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Services
{
    // Araç işlemleri için servis sınıfımızı oluşturuyoruz.
    public class CarService : ICarService
    {
        // Veritabanı işlemlerini tek noktadan yönetmek için UnitOfWork desenini kullanıyoruz.
        private readonly IUnitOfWork _unitOfWork;

        // Constructor ile dışarıdan IUnitOfWork örneği alıyoruz.
        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Sistemde kayıtlı olan ve silinmemiş tüm araçları getiriyoruz.
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _unitOfWork.CarRepository.FindAsync(c => !c.IsDeleted);
        }

        // Plaka numarasına göre aracı getiriyoruz. Silinmemiş olmasına dikkat ediyoruz.
        public async Task<Car> GetCarByPlateNumberAsync(string plateNumber)
        {
            return (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber && !c.IsDeleted==true)).FirstOrDefault();
        }

        // Yeni araç kaydı oluşturuyoruz.
        public async Task<Result<CarDto>> CreateCarAsync(CarDto carDto)
        {
            // Yeni araç entity'si oluşturuyoruz.
            var car = new Car
            {
                Id = Guid.NewGuid(),
                Brand = carDto.Brand,
                Model = carDto.Model,
                Year = carDto.Year,
                PlateNumber = carDto.PlateNumber,
                DailyRate = carDto.DailyRate,
                IsAvailable = true,       // Yeni araç varsayılan olarak müsaittir.
                IsDeleted = false
            };

            // Aracı veritabanına ekliyoruz.
            await _unitOfWork.CarRepository.AddAsync(car);
            await _unitOfWork.SaveChangesAsync(); // Değişiklikleri kaydediyoruz.

            // DTO'ya güncellenmiş verileri geri yazıyoruz.
            carDto.Id = car.Id;
            carDto.IsAvailable = car.IsAvailable;

            // Başarılı sonuçla birlikte DTO'yu döndürüyoruz.
            return new Result<CarDto>
            {
                IsSucced = true,
                Message = "Car created successfully",
                Data = carDto
            };
        }

        // Aracın tüm bilgilerini güncelliyoruz.
        public async Task UpdateCarAsync(string plateNumber, UpdateCarDto carDto)
        {
            // Güncellenecek aracı plakaya göre buluyoruz.
            var car = (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber && !c.IsDeleted)).FirstOrDefault();
            if (car == null)
            {
                throw new Exception("Car not found or has been deleted.");
            }

            // Yeni bilgileri araca yazıyoruz.
            car.Brand = carDto.Brand;
            car.Model = carDto.Model;
            car.Year = carDto.Year;
            car.DailyRate = carDto.DailyRate;
            car.IsAvailable = carDto.IsAvailable;

            // Veritabanında güncelliyoruz.
            await _unitOfWork.CarRepository.UpdateAsync(car);
        }

        // Aracın sadece bazı alanlarını (partial update) güncelliyoruz.
        public async Task PatchCarAsync(string plateNumber, PatchCarDto patchCarDto)
        {
            var car = (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber && !c.IsDeleted)).FirstOrDefault();
            if (car == null)
            {
                throw new Exception("Car not found or has been deleted.");
            }

            // Sadece gönderilen alanları güncelliyoruz.
            if (patchCarDto.IsAvailable.HasValue)
            {
                car.IsAvailable = patchCarDto.IsAvailable.Value;
            }

            if (patchCarDto.DailyRate.HasValue)
            {
                car.DailyRate = patchCarDto.DailyRate.Value;
            }

            await _unitOfWork.CarRepository.UpdateAsync(car);
        }

        // Aracı tamamen silmiyoruz, sadece 'soft delete' uygulayıp IsDeleted true yapıyoruz.
        public async Task DeleteCarAsync(string plateNumber)
        {
            var car = (await _unitOfWork.CarRepository.FindAsync(c => c.PlateNumber == plateNumber)).FirstOrDefault();
            if (car != null)
            {
                car.IsDeleted = true;
                await _unitOfWork.CarRepository.UpdateAsync(car);
            }
        }

        // Kiralanabilir olan (müsait ve silinmemiş) araçları getiriyoruz.
        public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            return await _unitOfWork.CarRepository.FindAsync(c => c.IsAvailable && !c.IsDeleted);
        }
    }
}
