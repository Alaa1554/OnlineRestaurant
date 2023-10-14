using OnlineRestaurant.Services;

namespace OnlineRestaurant.Models
{
    public class WishList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<Meal> Meals { get; set; }
        public ApplicationUser User { get; set; }
        public IEnumerable<WishListMeals> wishListMeals { get; set; }
        
    }
}
