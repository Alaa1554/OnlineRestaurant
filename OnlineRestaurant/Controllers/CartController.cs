using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {   private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;
        public CartController(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetCart()
        {
          var cart= _cartService.GetCart();
            return Ok(cart);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> AddMealToCart(int id)
        {
          var meal=  await _context.Meals.FindAsync(id);
            if (meal == null)
                return NotFound($"There is no Meal With Id :{id}");
           var addedmeal= _cartService.AddToCart(meal);
            if(!string.IsNullOrEmpty(addedmeal))
                return BadRequest(addedmeal);
            var Cart = _cartService.GetCart();
            var Message = "تم اضافه الوجبه بنجاح في السله";
            return Ok(new { Cart, Message  });
            
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMeal(int id)
        {

           var deletedmeal= _cartService.RemoveFromCart(id);
            if (!string.IsNullOrEmpty(deletedmeal))
                return BadRequest(deletedmeal);
            var cart = _cartService.GetCart();
            return Ok(cart);

        }
    }
}
