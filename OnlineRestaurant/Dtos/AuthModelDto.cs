namespace OnlineRestaurant.Dtos
{
    public class AuthModelDto
    {
        public string Message { get ; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set;}
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }

    }
}
