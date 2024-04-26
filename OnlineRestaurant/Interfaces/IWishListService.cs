
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IWishListService
    {
        Task<WishListMealView> GetWishlistAsync(string userid, PaginateDto dto);
        Task<string> AddToWishList (string token, int mealid);
        Task<string> RemoveFromWishList(string token, int mealid);
      


    }
}
