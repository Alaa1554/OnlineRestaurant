using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateCouponDto
    {
        [MaxLength(10)]
        public string? CouponCode { get; set; }
        [Range(0.1, 100)]
        public decimal? DiscountPercentage { get; set; }
    }
}
