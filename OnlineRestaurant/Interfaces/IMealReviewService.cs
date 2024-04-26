using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealReviewService
    {
        IEnumerable<MealReviewView> GetReviews(int id, PaginateDto dto);
        Task<MealReviewView> CreateReview(string token,MealReview comment);
        Task<MealReviewView> UpdateReviewAsync(int id, UpdateReviewDto dto);
        Task<MealReviewView> DeleteReviewAsync(int id);
    }
}
