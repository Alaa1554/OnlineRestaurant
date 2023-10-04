using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealReviewService
    {
        Task<IEnumerable<MealReviewView>> GetReviewsAsync(int id);
        Task<MealReview> GetReviewByIdAsync(int id);
        Task<MealReviewView> CreateReview(MealReview comment);
        MealReviewView UpdateReviewAsync(MealReview comment, UpdateReviewDto dto);
        MealReviewView DeleteReviewAsync(MealReview comment);
    }
}
