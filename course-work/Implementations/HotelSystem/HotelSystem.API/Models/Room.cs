using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        [MaxLength(20)]
        public string? RoomType { get; set; }

        [Required]
        public decimal PricePerNight { get; set; }

        [Required]
        public int Capacity { get; set; }

        public Hotel Hotel { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
