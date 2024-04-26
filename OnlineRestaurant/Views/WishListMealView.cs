
namespace OnlineRestaurant.Views
{
    public class WishListMealView
    {
        public int WishListId { get; set; }
        public IEnumerable<MealView> Meals { get; set; }
    }
}
