namespace OnlineRestaurant.Dtos
{
    public class OrderDto
    {
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public int AddressId { get; set; }
        public List<StaticAdditionOrderDto>? StaticAdditionOrders { get; set; }
        public List<MealOrderDto> MealOrders { get; set; }
    }
}
