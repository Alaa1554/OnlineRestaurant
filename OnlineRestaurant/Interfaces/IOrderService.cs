using OnlineRestaurant.Dtos;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces;



public interface IOrderService
{
    Task<OrderView> AddOrderAsync (string token,OrderDto orderDto);
    Task<OrderView> GetOrderByIdAsync(string id);
}
