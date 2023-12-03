namespace OnlineRestaurant.Models
{
    public class OrderStaticAddition
    {
       
        public int StaticMealAdditionId { get; set; }
        public string OrderId { get; set; }
        public int Amount { get; set; }
        public StaticMealAddition StaticMealAddition { get; set; }
        public Order Order { get; set; }
    }
}
