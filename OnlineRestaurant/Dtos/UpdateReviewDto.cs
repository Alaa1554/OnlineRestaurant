using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateReviewDto
    {
        [MaxLength(100)]
        public string? Text { get; set; }
      
        public decimal? Rate { get; set; }
    }
}
