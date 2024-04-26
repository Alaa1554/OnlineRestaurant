using OnlineRestaurant.Models;

namespace OnlineRestaurant.Dtos
{
    public class InsertOrderDto
    {
        public OrderDto OrderDto { get; set; }
        public Address Address { get; set; }
        public string UserId { get; set; }
    }
}
