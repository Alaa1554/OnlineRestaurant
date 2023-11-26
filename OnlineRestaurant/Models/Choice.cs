

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? Price { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public MealAddition MealAddition { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public int MealAdditionId { get; set; }
    }
}
