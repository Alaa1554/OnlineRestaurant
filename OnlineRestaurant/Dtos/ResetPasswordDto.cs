using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Password must be complex.")]
        public string NewPassword { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
