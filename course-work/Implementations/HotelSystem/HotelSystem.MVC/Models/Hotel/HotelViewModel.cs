using System.ComponentModel.DataAnnotations;

namespace HotelSystem.MVC.Models.Hotel
{
    public class HotelViewModel
    {
        public int HotelId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Range(1, 5)]
        public int StarRating { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
}
