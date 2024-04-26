using OnlineRestaurant.Models;

namespace OnlineRestaurant.Dtos
{
    public class OrderDto
    {
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public int AddressId { get; set; }
        public List<OrderStaticAddition>? StaticAdditionOrders { get; set; }
        public List<OrderMeal> MealOrders { get; set; }
    }
}
