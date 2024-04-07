using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using System.IdentityModel.Tokens.Jwt;

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
            var userId = _authService.GetUserId(token);
            if(!await _userManager.Users.AnyAsync(c => c.Id == userId))
                return NotFound("No User is Found!");
            var orders = await _orderService.GetAllUserOrders(userId,paginate);
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
