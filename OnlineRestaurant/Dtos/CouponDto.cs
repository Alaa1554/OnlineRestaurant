
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
   
    public class CouponDto
    {
        [Required]
        [MaxLength(10)]
        public string CouponCode { get; set; }
        [Required]
        [Range(0.1, 100)]
        public decimal DiscountPercentage { get; set; }
    }
}
