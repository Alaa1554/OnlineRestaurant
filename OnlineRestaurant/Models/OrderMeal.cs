namespace OnlineRestaurant.Models
{
    public class OrderMeal
    {
        public int MealId { get; set; }
        public string OrderId { get; set; }
        public string Addition { get; set; }
        public int Amount { get; set; }
        public Meal Meal { get; set; }
        public Order Order { get; set; }
    }
}
