namespace OnlineRestaurant.Views
{
    public class OrderMealView
    {
        public int MealId { get; set; }
        public string MealName { get; set; }
        public string Addition { get; set;}
        public int Amount { get; set; }
        public string MealImgUrl { get; set; }
        public decimal MealPrice { get; set; }
    }
}
