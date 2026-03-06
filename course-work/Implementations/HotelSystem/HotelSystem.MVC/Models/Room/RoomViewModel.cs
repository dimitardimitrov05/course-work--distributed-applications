using System.ComponentModel.DataAnnotations;

namespace HotelSystem.MVC.Models.Room
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoomType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double PricePerNight { get; set; }

        public int Capacity { get; set; }

        public string? HotelName { get; set; }
    }
}
