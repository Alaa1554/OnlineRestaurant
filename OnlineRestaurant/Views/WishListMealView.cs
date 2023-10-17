using OnlineRestaurant.Models;
using OnlineRestaurant.Services;

namespace OnlineRestaurant.Views
{
    public class WishListMealView
    {
        public int WishListId { get; set; }
        public List<MealView> Meals { get; set; }
    }
}
