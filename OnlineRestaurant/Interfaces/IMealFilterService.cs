using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealFilterService
    {
        Task<IEnumerable<MealView>> Filter (MealFilter filter);
       
    }
}
