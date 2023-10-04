using Microsoft.Build.Framework;

namespace OnlineRestaurant.Dtos
{
    public class AddRoleDto
    {
        [Required]
        public string Role { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
