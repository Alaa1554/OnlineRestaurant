using OnlineRestaurant.Services;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IWishListService
    {
        Task<IEnumerable<WishListMealView>> GetWishlistAsync(string token);
        Task<string> AddToWishList (string token, int mealid);
        Task<string> RemoveFromWishList(string token, int mealid);
        string GetUserId(string token);


    }
}
