using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class CouponCodeDto
    {
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }
    }
}
