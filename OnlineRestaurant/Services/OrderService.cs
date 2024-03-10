
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
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
                NumOfMeals=orderDto.MealOrders.Count(),
                NumOfStaticMealAdditions=orderDto.StaticAdditionOrders.Count(),

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
                    MealImgUrl= Path.Combine("https://localhost:7166", "images", c.Meal.MealImgUrl),
                    MealPrice=c.Meal.Price
                    
                })).ToList(),
                StaticAdditions = OrderStaticAdditions.Include(c => c.StaticMealAddition).GroupBy(c => c.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderStaticAdditionView
                {
                    Id = c.StaticMealAdditionId,
                    Amount = c.Amount,
                    StaticAdditionName = c.StaticMealAddition.Name,
                    StaticAdditionImgUrl= Path.Combine("https://localhost:7166", "images", c.StaticMealAddition.AdditionUrl),
                    StaticAdditionPrice=c.StaticMealAddition.Price

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
                    MealImgUrl = Path.Combine("https://localhost:7166", "images", c.Meal.MealImgUrl),
                    MealPrice=c.Meal.Price
                    
                })).ToList(),
                StaticAdditions = OrderStaticAdditions.Include(c => c.StaticMealAddition).GroupBy(c => c.OrderId).AsEnumerable().SelectMany(o => o.Select(c => new OrderStaticAdditionView
                {
                    Id = c.StaticMealAdditionId,
                    Amount = c.Amount,
                    StaticAdditionName = c.StaticMealAddition.Name,
                    StaticAdditionImgUrl = Path.Combine("https://localhost:7166", "images", c.StaticMealAddition.AdditionUrl),
                    StaticAdditionPrice = c.StaticMealAddition.Price
                })).ToList()
            };
        }
        public async Task<IEnumerable<UserOrderView>> GetAllUserOrders(string token,PaginateDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var user = await _userManger.FindByIdAsync(userId);
            if (!await _userManger.Users.AnyAsync(c => c.Id == userId))
            {
                return Enumerable.Empty<UserOrderView>();
            }
            var orders = _context.Orders.Include(o => o.OrderMeals).Include(o => o.OrderStaticAdditions).Where(o=>o.UserId == userId).Paginate(dto.Page, dto.Size).Select(o=>new UserOrderView
            {
                Id=o.Id,
                Date=o.Date,
                DepartmentNum = o.DepartmentNum,
                City = o.City,
                IsPaid = o.IsPaid,
                PaymentMethod = o.PaymentMethod,
                PhoneNumber = o.PhoneNumber,
                Status = o.Status,
                Street = o.Street,
                TotalCost = o.TotalCost,
                NumOfMeals=o.NumOfMeals,
                NumOfStaticMealAdditions=o.NumOfStaticMealAdditions,
            }).OrderByDescending(o=>o.Date).ToList();
            return orders;
        }
        public IEnumerable<AdminOrderView> GetAllOrders(PaginateDto dto)
        {
            var orders =_context.Orders.Include(o => o.User).Include(o => o.OrderMeals).Include(o => o.OrderStaticAdditions).Paginate(dto.Page, dto.Size).Select(o => new AdminOrderView
            {
                Id = o.Id,
                Date = o.Date,
                DepartmentNum = o.DepartmentNum,
                City = o.City,
                IsPaid = o.IsPaid,
                PaymentMethod = o.PaymentMethod,
                TotalCost = o.TotalCost,
                PhoneNumber = o.PhoneNumber,
                Status = o.Status,
                Street = o.Street,
                UserImg = o.User.UserImgUrl==null?null: Path.Combine("https://localhost:7166", "images", o.User.UserImgUrl),
                UserName = o.User.UserName,
                NumOfMeals = o.NumOfMeals,
                NumOfStaticMealAdditions = o.NumOfStaticMealAdditions,
            }).OrderByDescending(o => o.Date).ToList();
            return orders;
        }
        public async Task<string> ChangeOrderStatus(OrderStatusDto dto)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == dto.OrderId);
            if (order == null)
                return "لم يتم العثور علي اي طلب" ;
            order.Status = dto.Status;
            await _context.SaveChangesAsync();
            return string.Empty;
            
        }
        public async Task<string> ConfirmPayment(ConfirmPaymentDto dto)
        {
            
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == dto.OrderId);
            if (order == null)
                return "لم يتم العثور علي اي طلب";
            if (order.PaymentMethod.ToLower().Trim() == "credit")
                return "تم تاكيد الدفع مسبقا";
            if (order.Status.ToLower().Trim() != "delivered")
                return "يجب ان يتم توصيل الاوردر قبل تاكيد الدفع";
            order.IsPaid=true;
            await _context.SaveChangesAsync();
            return string.Empty;
        }
    }
}
