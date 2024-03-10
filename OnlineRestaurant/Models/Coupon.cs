using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string CouponCode { get; set; }
        [Range(0.1, 100)]
        public decimal DiscountPercentage { get; set; }
    }
}
