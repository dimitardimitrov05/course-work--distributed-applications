using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public decimal TotalPrice { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        public Room Room { get; set; }

        public ApplicationUser User { get; set; }
    }
}
