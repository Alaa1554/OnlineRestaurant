using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class EmailDto
    {
        [Required]
        public string Email { get; set; }
    }
}
