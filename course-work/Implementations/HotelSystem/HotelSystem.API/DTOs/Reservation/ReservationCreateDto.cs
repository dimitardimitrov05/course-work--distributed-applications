using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Reservation
{
    public class ReservationCreateDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }
    }
}
