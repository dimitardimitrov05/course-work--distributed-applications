using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Reservation
{
    public class ReservationUpdateDto
    {
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
    }
}
