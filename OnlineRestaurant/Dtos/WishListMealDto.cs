using OnlineRestaurant.Models;
using OnlineRestaurant.Services;

namespace OnlineRestaurant.Dtos
{
    public class WishListMealDto
    {
        public int WishListId { get; set; }

        public IEnumerable<WishListMeal> Meals { get; set; }
    }
}
