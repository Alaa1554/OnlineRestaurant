using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateReviewDto
    {
        [Required,MaxLength(100)]
        public string? Text { get; set; }
        [Required]
        public decimal? Rate { get; set; }
    }
}
