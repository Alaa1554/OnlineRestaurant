

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Range(0,int.MaxValue,ErrorMessage ="Price must be more than -1")]
        public decimal? Price { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public MealAddition MealAddition { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public int MealAdditionId { get; set; }
    }
}
