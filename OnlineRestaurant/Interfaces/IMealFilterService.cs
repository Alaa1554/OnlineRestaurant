using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealFilterService
    {
        Task<MealFilterView> Filter (string? token,MealFilter filter);
       
    }
}
