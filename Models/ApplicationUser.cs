using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestApi.Models
{
    public class ApplicationUser:IdentityUser
    {
        [MaxLength(50)]
        public required string FristName { get; set; }
        [MaxLength(50)]
        public required string LastName { get; set; }

    }
}
