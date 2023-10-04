using Microsoft.Build.Framework;

namespace OnlineRestaurant.Dtos
{
    public class TokenRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
