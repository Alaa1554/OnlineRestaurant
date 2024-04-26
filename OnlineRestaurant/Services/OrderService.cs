using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public OrderService(UserManager<ApplicationUser> userManger, ApplicationDbContext context, IAuthService authService, IMapper mapper)
        {
            _userManger = userManger;
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<OrderView> AddOrderAsync(string token, OrderDto orderDto)
        {
            var userId = _authService.GetUserId(token);
            var user = await _userManger.FindByIdAsync(userId);
            if (!await _userManger.Users.AnyAsync(c => c.Id == userId))
                return new OrderView { Message = "No User is Found!" };
            var address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == orderDto.AddressId);
            if (address == null)
                return new OrderView { Message = "لم يتم العثور علي اي عنوان" };
            var order = _mapper.Map<Order>(new InsertOrderDto {Address=address,OrderDto=orderDto,UserId=userId});
            await _context.AddAsync(order);
            foreach(var meal in orderDto.MealOrders)
            {
                if(!await _context.Meals.AnyAsync(c=>c.Id == meal.MealId))
                    return new OrderView { Message = $"No Meal Found With Id:{meal.MealId}" };
                meal.OrderId = order.Id;
                await _context.AddAsync(meal);
            }
            if(orderDto.StaticAdditionOrders!=null)
            {
                foreach(var staticAddition in orderDto.StaticAdditionOrders)
                {
                    if (!await _context.StaticAdditions.AnyAsync(c => c.Id == staticAddition.StaticMealAdditionId))
                        return new OrderView { Message = $"No StaticAddition Found With Id:{staticAddition.StaticMealAdditionId}" };
                    staticAddition.OrderId = order.Id;
                    await _context.AddAsync(staticAddition);
                }
            }
            await _context.SaveChangesAsync();
            var returnOrder=await _context.Orders.Include(o=>o.OrderMeals).ThenInclude(om=>om.Meal).Include(o=>o.OrderStaticAdditions).ThenInclude(os=>os.StaticMealAddition).SingleOrDefaultAsync(o=>o.Id==order.Id);
            return _mapper.Map<OrderView>(returnOrder);
        }

        public async Task<OrderView> GetOrderByIdAsync(string id)
        {
            var order = await _context.Orders.Include(o => o.OrderMeals).ThenInclude(om => om.Meal).Include(o => o.OrderStaticAdditions).ThenInclude(os => os.StaticMealAddition).SingleOrDefaultAsync(o => o.Id ==id);
            if (order == null)
                return new OrderView { Message = "لم يتم العثور علي اي طلب" };
            return _mapper.Map<OrderView>(order);
        }
        public async Task<IEnumerable<UserOrderView>> GetAllUserOrders(string userId,PaginateDto dto)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (!await _userManger.Users.AnyAsync(c => c.Id == userId))
                return new List<UserOrderView> { new UserOrderView { Id="-1"} };
            return _mapper.Map<IEnumerable<UserOrderView>>(_context.Orders.Include(o => o.OrderMeals).Include(o => o.OrderStaticAdditions).Where(o=>o.UserId == userId).Paginate(dto.Page, dto.Size).OrderByDescending(o=>o.Date));
        }
        public IEnumerable<AdminOrderView> GetAllOrders(PaginateDto dto)
        {
            return _mapper.Map<IEnumerable<AdminOrderView>>(_context.Orders.Include(o => o.User).Include(o => o.OrderMeals).Include(o => o.OrderStaticAdditions).Paginate(dto.Page, dto.Size).OrderByDescending(o => o.Date));
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
