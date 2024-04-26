using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Dtos
{
    public class InsertMealDto
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "This Field is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be more than -1")]
        public decimal Price { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public int ChefId { get; set; }
        public int CategoryId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "OldPrice must be more than -1")]
        public decimal? OldPrice { get; set; }
        public IFormFile MealImg { get; set; }
    }
}
