using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IFilterStrategy
    {
        bool CanApply(MealFilter filter);
        IEnumerable<MealView> ApplyFilter();
    }
}
