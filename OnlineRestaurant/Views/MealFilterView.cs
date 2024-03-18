namespace OnlineRestaurant.Views
{
    public class MealFilterView
    {
        public IEnumerable<MealView> Meals { get; set; }
        public int NumOfPages { get; set; }
    }
}
