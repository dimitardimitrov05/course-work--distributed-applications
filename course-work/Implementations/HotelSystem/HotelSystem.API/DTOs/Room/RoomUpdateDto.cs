using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Room
{
    public class RoomUpdateDto
    {
        [MaxLength(10)]
        public string? RoomNumber { get; set; }

        [MaxLength(20)]
        public string? RoomType { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? PricePerNight { get; set; }

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; }
    }
}
