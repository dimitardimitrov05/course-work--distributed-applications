using System.ComponentModel.DataAnnotations;

namespace HotelSystem.MVC.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
