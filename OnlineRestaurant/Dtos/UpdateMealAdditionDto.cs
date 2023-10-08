using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateMealAdditionDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public UpdateChoiceDto? Choice { get; set; }
        public int? MealId { get; set; }
    }
}
