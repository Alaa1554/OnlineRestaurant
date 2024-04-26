using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class OrderMeal
    {
        public int MealId { get; set; }
        [ValidateNever,JsonIgnore]
        public string OrderId { get; set; }
        public string Addition { get; set; }
        public int Amount { get; set; }
        [ValidateNever, JsonIgnore]
        public Meal Meal { get; set; }
        [ValidateNever, JsonIgnore]
        public Order Order { get; set; }
    }
}
