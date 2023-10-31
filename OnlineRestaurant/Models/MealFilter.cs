namespace OnlineRestaurant.Models
{
    public class MealFilter
    {
        public string? Category { get; set; }
        public string? MealName { get; set; }
        public string? ChefName { get; set; }
        public int? FromPrice { get; set; }
        public int? ToPrice { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string? OrderMeal { get; set; }
        
     
    }
}
