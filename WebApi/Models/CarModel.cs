using System.ComponentModel.DataAnnotations;

namespace Rent.WebApi.Models
{
    public class CarModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        [Required]
        public string PlateNumber { get; set; }
        public decimal DailyRate { get; set; }

    }
}
