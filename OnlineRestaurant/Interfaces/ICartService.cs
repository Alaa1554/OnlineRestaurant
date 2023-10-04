using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface ICartService
    {
        Cart GetCart();
        string AddToCart(Meal meal);
        string RemoveFromCart(int id);
    }
}
