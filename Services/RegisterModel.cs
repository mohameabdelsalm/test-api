using System.ComponentModel.DataAnnotations;

namespace TestApi.Services
{
    public class RegisterModel
    {
        [MaxLength(128)]
        public  string FristName { get; set; }
        [Required, MaxLength(50)]
        public  string LastName { get; set; }
        [Required, MaxLength(100)]

        public  string UserName { get; set; }

        [Required, MaxLength(100)]

        public  string Email { get; set; }

        [Required, MaxLength(256)]

        public  string Password { get; set; }
    }
}