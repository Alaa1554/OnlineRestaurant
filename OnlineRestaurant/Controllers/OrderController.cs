using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IOrderService orderService, ApplicationDbContext context, IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _context = context;
            _authService = authService;
            _userManager = userManager;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrderAsync([FromHeader] string token, [FromBody] OrderDto dto)
        {
            var order = await _orderService.AddOrderAsync(token, dto);
            if(!string.IsNullOrEmpty(order.Message))
                return NotFound(order.Message);
         return Ok(order);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderByIdAsync(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (!string.IsNullOrEmpty(order.Message))
                return NotFound(order.Message);
            return Ok(order);

        }
        [HttpGet("GetAllUserOrders")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> GetAllUserOrdersAsync([FromHeader] string token,[FromQuery] PaginateDto paginate)
        {
            var userId = _authService.GetUserId(token);
            var orders = await _orderService.GetAllUserOrders(userId,paginate);
            if(orders.Any(c=>c.Id=="-1"))
                return NotFound("No user is found");
            bool nextPage = false;
            if (orders.Count() > paginate.Size)
            {
                orders = orders.Take(orders.Count() - 1);
                nextPage = true;
            }
            var numOfUserOrders = await _context.Orders.CountAsync(c => c.UserId == userId);
            var numOfPages = (int)Math.Ceiling((decimal)numOfUserOrders / paginate.Size);
            return Ok(new { Orders = orders, NextPage = nextPage,NumOfPages=numOfPages,NumOfUserOrders=numOfUserOrders});
        }
        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery]PaginateDto paginate)
        {
            var orders =_orderService.GetAllOrders(paginate);
            bool nextPage = false;
            if (orders.Count() > paginate.Size)
            {
                orders = orders.Take(orders.Count() - 1);
                nextPage = true;
            }
            var numOfOrders = await _context.Orders.CountAsync();
            var numOfPages = (int)Math.Ceiling((decimal)numOfOrders / paginate.Size);
            return Ok(new { Orders = orders, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpPut("ChangeOrderStatus")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ChangeOrderStatusAsync([FromBody] OrderStatusDto orderStatus)
        {
            var result = await _orderService.ChangeOrderStatus(orderStatus);
            if (!string.IsNullOrEmpty(result))
                return NotFound(result);
            return Ok("تم تغيير حاله الاوردر بنجاح");
        }
        [HttpPost("ConfirmPayment")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ConfirmPaymentAsync(ConfirmPaymentDto confirmPayment)
        {
            var result = await _orderService.ConfirmPayment(confirmPayment);
            if(!string.IsNullOrEmpty(result))
                return NotFound(result);
            return Ok("تم تاكيد الدفع");
        }
    }
}
