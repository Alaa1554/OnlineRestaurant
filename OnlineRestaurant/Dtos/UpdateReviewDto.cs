using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateReviewDto
    {
        [MaxLength(100)]
        public string Text { get; set; }
        [Range(0.1,5)]     
        public decimal Rate { get; set; }
    }
}
