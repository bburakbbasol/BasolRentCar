namespace Rent.WebApi.Models
{
    public class PatchCarRequest
    {
        public bool? IsAvailable { get; set; }
        public decimal? DailyRate { get; set; }
    }
}
