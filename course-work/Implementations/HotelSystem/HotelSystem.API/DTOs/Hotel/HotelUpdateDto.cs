using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Hotel
{
    public class HotelUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [Range(1, 5)]
        public int? StarRating { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
}
