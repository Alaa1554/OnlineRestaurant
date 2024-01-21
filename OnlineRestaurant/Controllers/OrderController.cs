using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderAsync([FromHeader] string token, [FromBody] OrderDto order)
        {
            var orderview = await _orderService.AddOrderAsync(token, order);
            if(!string.IsNullOrEmpty(orderview.Message))
                return NotFound(orderview.Message);
         return Ok(orderview);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (!string.IsNullOrEmpty(order.Message))
                return NotFound(order.Message);
            return Ok(order);

        }
        [HttpGet("GetAllUserOrders")]
        public async Task<IActionResult> GetAllUserOrdersAsync([FromHeader] string token)
        {
            var orders = await _orderService.GetAllUserOrders(token);
            if (!orders.Any())
                return NotFound("No User is Found!");
            return Ok(orders);
        }

    }
}
