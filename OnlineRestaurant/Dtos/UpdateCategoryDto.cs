using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public IFormFile? CategoryImg { get; set; }
    }
}
