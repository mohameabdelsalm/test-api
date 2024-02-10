using System.ComponentModel.DataAnnotations;

namespace TestApi.Services
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
    }
}