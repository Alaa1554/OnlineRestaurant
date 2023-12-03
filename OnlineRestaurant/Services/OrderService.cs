
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineRestaurant.Services
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly ApplicationDbContext _context;

        public OrderService(UserManager<ApplicationUser> userManger, ApplicationDbContext context)
        {
            _userManger = userManger;
            _context = context;
        }

        public async Task<OrderView> AddOrderAsync(string token, OrderDto orderDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var user = await _userManger.FindByIdAsync(userId);
            if (!await _userManger.Users.AnyAsync(c => c.Id == userId))
            {
                return new OrderView { Message = "No User is Found!" };
            }
            var address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == orderDto.AddressId);
            if (address == null)
            {
                return new OrderView { Message = "لم يتم العثور علي اي عنوان" };
            }
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                Status = "Processing",
                StatusDate = DateTime.Now,
                Date = DateTime.Now,
                PaymentMethod = orderDto.PaymentMethod,
                UserId = userId,
                DepartmentNum = address.DepartmentNum,
                City = address.City,
                PhoneNumber = address.PhoneNumber,
                Street = address.Street,
                TotalCost = orderDto.TotalPrice,

            };
            if(orderDto.PaymentMethod.ToLower().Trim()=="credit")
                order.IsPaid= true;
            else
                order.IsPaid= false;
            await _context.AddAsync(order);
            foreach(var meal in orderDto.MealOrders)
            {
                if(!await _context.Meals.AnyAsync(c=>c.Id == meal.Id))
                {
                    return new OrderView { Message = $"No Meal Found With Id:{meal.Id}" };
                }
                var ordermeal = new OrderMeal
                {
                    MealId = meal.Id,
                    OrderId = order.Id,
                    Addition = meal.Addition,
                    Amount = meal.Amount,
                };
                await _context.AddAsync(ordermeal);
            }
            if(orderDto.StaticAdditionOrders!=null)
            {
                foreach(var staticAddition in orderDto.StaticAdditionOrders)
                {
                    if (!await _context.StaticAdditions.AnyAsync(c => c.Id == staticAddition.Id))
                    {
                        return new OrderView { Message = $"No StaticAddition Found With Id:{staticAddition.Id}" };
                    }
                    var OrderStaticAddition = new OrderStaticAddition
                    {
                        OrderId = order.Id,
                        Amount = staticAddition.Amount,
                        StaticMealAdditionId = staticAddition.Id
                    };
                    await _context.AddAsync(OrderStaticAddition);
                }
            }
            await _context.SaveChangesAsync();
            var OrderMeals = _context.OrderMeals.Where(o => o.OrderId == order.Id);
            var OrderStaticAdditions = _context.OrdersStaticAdditions.Where(o=>o.OrderId == order.Id);
            return new OrderView
            {
                Id = order.Id,
                Date = order.Date,
                DepartmentNum = order.DepartmentNum,
                City = order.City,
                IsPaid = order.IsPaid,
                PaymentMethod = order.PaymentMethod,
                PhoneNumber = order.PhoneNumber,
                Status = order.Status,
                StatusDate = order.StatusDate,
                Street = order.Street,
                TotalCost = order.TotalCost,
                Meals = OrderMeals.Include(c => c.Meal).GroupBy(m => m.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderMealView
                {
                    Id = c.MealId,
                    Addition = c.Addition,
                    MealName = c.Meal.Name,
                    Amount = c.Amount,
                    MealImgUrl=c.Meal.MealImgUrl
                })).ToList(),
                StaticAdditions = OrderStaticAdditions.Include(c => c.StaticMealAddition).GroupBy(c => c.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderStaticAdditionView
                {
                    Id = c.StaticMealAdditionId,
                    Amount = c.Amount,
                    StaticAdditionName = c.StaticMealAddition.Name,
                    StaticAdditionImgUrl=c.StaticMealAddition.AdditionUrl
                })).ToList()
            };
            
        }

        public async Task<OrderView> GetOrderByIdAsync(string id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o=> o.Id == id);
            if (order == null)
            {
                return new OrderView { Message = "لم يتم العثور علي اي طلب" };
            }
            var OrderMeals = _context.OrderMeals.Where(o => o.OrderId == order.Id);
            var OrderStaticAdditions = _context.OrdersStaticAdditions.Where(o => o.OrderId == order.Id);
            return new OrderView
            {
                Id = order.Id,
                Date = order.Date,
                DepartmentNum = order.DepartmentNum,
                City = order.City,
                IsPaid = order.IsPaid,
                PaymentMethod = order.PaymentMethod,
                PhoneNumber = order.PhoneNumber,
                Status = order.Status,
                StatusDate = order.StatusDate,
                Street = order.Street,
                TotalCost = order.TotalCost,
                Meals = OrderMeals.Include(c => c.Meal).GroupBy(m => m.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderMealView
                {
                    Id = c.MealId,
                    Addition = c.Addition,
                    MealName = c.Meal.Name,
                    Amount = c.Amount,
                    MealImgUrl = c.Meal.MealImgUrl
                })).ToList(),
                StaticAdditions = OrderStaticAdditions.Include(c => c.StaticMealAddition).GroupBy(c => c.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderStaticAdditionView
                {
                    Id = c.StaticMealAdditionId,
                    Amount = c.Amount,
                    StaticAdditionName = c.StaticMealAddition.Name,
                    StaticAdditionImgUrl = c.StaticMealAddition.AdditionUrl
                })).ToList()
            };
        }
    }
}
