using OnlineRestaurant.Models;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class MealByNameView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        public string Image { get; set; }
        public string ChefName { get; set; }
        public string CategoryName { get; set; }
        public decimal Rate { get; set; }
        public int NumOfRates { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IsFavourite { get; set; }
        public List<StaticMealAddition> StaticMealAdditions { get; set; }
        public List<AdditionView> MealAdditions { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? OldPrice { get; set; }
        public List<MealReviewView> Reviews { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
    }
}
