using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class OrderStaticAddition
    {
       
        public int StaticMealAdditionId { get; set; }
        [ValidateNever, JsonIgnore]
        public string OrderId { get; set; }
        public int Amount { get; set; }
        [ValidateNever, JsonIgnore]
        public StaticMealAddition StaticMealAddition { get; set; }
        [ValidateNever, JsonIgnore]
        public Order Order { get; set; }
    }
}
