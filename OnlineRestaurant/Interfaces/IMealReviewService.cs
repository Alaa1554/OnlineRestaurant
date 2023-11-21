using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealReviewService
    {
        Task<IEnumerable<MealReviewView>> GetReviewsAsync(int id);
        Task<MealReview> GetReviewByIdAsync(int id);
        Task<MealReviewView> CreateReview(string token,MealReview comment);
        Task<MealReviewView> UpdateReviewAsync(MealReview comment, UpdateReviewDto dto);
        Task<MealReviewView> DeleteReviewAsync(MealReview comment);
    }
}
