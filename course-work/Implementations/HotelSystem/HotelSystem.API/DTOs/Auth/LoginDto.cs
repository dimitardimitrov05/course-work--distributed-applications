using System.ComponentModel.DataAnnotations;

namespace HotelSystem.API.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
