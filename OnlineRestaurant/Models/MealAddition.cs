using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class MealAddition
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
       
        public List<Choice> Choices { get; set; }= new List<Choice>();
        [ValidateNever,JsonIgnore]
        public Meal Meal { get; set; }
        public int MealId { get; set; }
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}
