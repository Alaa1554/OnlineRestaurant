namespace OnlineRestaurant.Dtos
{
    public class UpdateAccountDto
    {
        public IFormFile? UserImg { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
