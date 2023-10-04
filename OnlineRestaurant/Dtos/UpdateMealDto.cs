using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateMealDto
    { 

        [MaxLength(100)]
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? MealImg { get; set; }
        public decimal? OldPrice { get; set; }
        public int? ChefId { get; set; }
        public int? CategoryId { get; set; }
    }
}
