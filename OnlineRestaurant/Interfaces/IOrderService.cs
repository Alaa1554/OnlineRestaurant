using OnlineRestaurant.Dtos;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces;



public interface IOrderService
{
    Task<OrderView> AddOrderAsync (string token,OrderDto orderDto);
    Task<OrderView> GetOrderByIdAsync(string id);
    Task<IEnumerable<UserOrderView>> GetAllUserOrders(string token, PaginateDto dto);
    IEnumerable<AdminOrderView> GetAllOrders(PaginateDto dto);
    Task<string> ChangeOrderStatus(OrderStatusDto dto);
    Task<string> ConfirmPayment(ConfirmPaymentDto dto);
}
