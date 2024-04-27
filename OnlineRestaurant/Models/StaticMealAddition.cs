using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class StaticMealAddition
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price must be more than -1")]
        public decimal Price { get; set; }
        [ValidateNever]
        public string AdditionUrl { get; set; }
        [NotMapped, Required(ErrorMessage = "This Field is Required"), JsonIgnore]
        public IFormFile? AdditionImg { get; set; }
        [ValidateNever,JsonIgnore]
        public List<Order> Orders { get; set; }
        [ValidateNever,JsonIgnore]
        public List<OrderStaticAddition> OrderStaticAdditions { get; set; }
       
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}
