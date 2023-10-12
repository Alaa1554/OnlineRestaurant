namespace OnlineRestaurant.Dtos
{
    public class UpdateStaticMealAdditionDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? AdditionImg { get; set; }
    }
}
