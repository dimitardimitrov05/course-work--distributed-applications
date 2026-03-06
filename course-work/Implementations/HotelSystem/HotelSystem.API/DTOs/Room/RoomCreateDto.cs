using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Room
{
    public class RoomCreateDto
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoomType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
