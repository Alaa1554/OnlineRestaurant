using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string CouponCode { get; set; }
        [Range(0.1, 100)]
        public decimal DiscountPercentage { get; set; }
        [NotMapped,JsonIgnore,ValidateNever]
        public string Message { get; set; }
    }
}
