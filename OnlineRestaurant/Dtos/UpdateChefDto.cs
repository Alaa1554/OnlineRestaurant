using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateChefDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public  IFormFile? ChefImg { get; set; }
        public int? CategoryId { get; set; }
    }
}
