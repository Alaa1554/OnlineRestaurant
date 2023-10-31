using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class GmailRegisterDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }

        public string? UserName { get; set; }
        [Required, StringLength(100)]
        public string Email { get; set; }
       
        public IFormFile? UserImg { get; set; }
    }
}
