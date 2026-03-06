using System.ComponentModel.DataAnnotations;

namespace HotelSystem.MVC.Models.Reservation
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }

        [Required]
        public int RoomId { get; set; }

        public string? HotelName { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public double TotalPrice { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
    }
}
