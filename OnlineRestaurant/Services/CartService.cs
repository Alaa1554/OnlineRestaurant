using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;


namespace OnlineRestaurant.Services
{ 
    public class CartService:ICartService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;

        public CartService(IHttpContextAccessor contextAccessor, ApplicationDbContext context)
        {
            _contextAccessor = contextAccessor;
            _context = context;
        }
        public Cart GetCart()
        {
            var cartjson = _contextAccessor.HttpContext?.Session.GetString("_cart");
            if( string.IsNullOrEmpty(cartjson) )
            {
                return new Cart();
            }
            else
            {
               return JsonConvert.DeserializeObject<Cart>(cartjson);
            }
            
        }
        private bool IsNewMeal(Meal meal)
        {
            var cart = GetCart();
            return cart.CartMeals ==null?true:!cart.CartMeals.Any(b=>b.Id == meal.Id);
        }
        public string AddToCart(Meal meal)
        {
            var cart = GetCart();
           
            if(IsNewMeal(meal))
            {
                cart.CartMeals.Add(meal);

                var cartjson = JsonConvert.SerializeObject(cart);
                _contextAccessor.HttpContext?.Session.SetString("_cart", cartjson);
                return null;
            }
            else
            {
                return "Meal Already in Cart";
            }
            
          
        }
        public string RemoveFromCart(int id)
        {
            var cart = GetCart();
            var meal = cart.CartMeals.Where(b => b.Id == id).FirstOrDefault();
            if( meal == null )
            {
                return $"No Meal is found with id:{id}!";
            }
            
            cart.CartMeals.Remove(meal);
            _contextAccessor.HttpContext?.Session.SetString("_cart", JsonConvert.SerializeObject(cart));
            return null;
        }
    }
}
