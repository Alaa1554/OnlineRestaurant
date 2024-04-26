using OnlineRestaurant.Models;

namespace OnlineRestaurant.Dtos
{
    public class MealReviewDto
    {
        public MealReview MealReview { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
