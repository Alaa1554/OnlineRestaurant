
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class RegisterModelDto
    {
        [Required,StringLength(50)]
        public string FirstName { get; set; }
        [Required, StringLength(50)]
        public string LastName { get; set; }
       
        public string UserName { get; set; }
        [Required, StringLength(100)]
        public string Email { get; set; }
        [Required, StringLength(250)]
        public string Password { get; set; }
        public IFormFile? UserImg { get; set; }
    }
}
