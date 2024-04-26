using OnlineRestaurant.Models;

namespace OnlineRestaurant.Dtos
{
    public class MapMealDto
    {
        public Meal Meal { get; set; }
        public List<StaticMealAddition> StaticMealAdditions { get; set; }
        public bool IsFavourite { get; set; }
    }
}
