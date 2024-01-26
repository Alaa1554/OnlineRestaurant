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
        public async Task<IActionResult> GetAllUserOrdersAsync([FromHeader] string token,[FromQuery] PaginateDto paginate)
        {
            var orders = await _orderService.GetAllUserOrders(token,paginate);
            if (!orders.Any())
                return NotFound("No User is Found!");
            return Ok(orders);
        }
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery]PaginateDto paginate)
        {
            var orders = await _orderService.GetAllOrders(paginate);
            return Ok(orders);
        }
        [HttpPut("ChangeOrderStatus")]
        public async Task<IActionResult> ChangeOrderStatusAsync([FromBody] OrderStatusDto orderStatus)
        {
            var Message = await _orderService.ChangeOrderStatus(orderStatus);
            if (!string.IsNullOrEmpty(Message))
                return NotFound(Message);
            return Ok("تم تغيير حاله الاوردر بنجاح");
        }
        [HttpPost("ConfirmPayment")]
        public async Task<IActionResult> ConfirmPaymentAsync(ConfirmPaymentDto confirmPayment)
        {
            var Message = await _orderService.ConfirmPayment(confirmPayment);
            if(!string.IsNullOrEmpty(Message))
                return NotFound(Message);
            return Ok("تم تاكيد الدفع");
        }
    }
}
