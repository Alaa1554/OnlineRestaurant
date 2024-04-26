
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Dtos
{
    public class AuthModelDto
    {
        IConfiguration configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get ; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set;}
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string UserImgUrl { get; set; }
       
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? FirstName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LastName { get; set; }
        
        public AuthModelDto(string Email, DateTime ExpiresOn,bool IsAuthenticated,List<string> Roles,string Token,string UserName,string UserImgUrl,string? FirstName,string? LastName) 
        {
            this.Email = Email;
            this.ExpiresOn = ExpiresOn;
            this.IsAuthenticated = IsAuthenticated;
            this.Roles = Roles;
            this.Token = Token;
            this.UserName = UserName;
            this.UserImgUrl = UserImgUrl==null?null:UserImgUrl.Contains("googleusercontent")?UserImgUrl:Path.Combine(configuration.GetSection("BaseUrl").Value, "images", UserImgUrl);
            this.FirstName = FirstName;
            this.LastName = LastName;
        }
        public AuthModelDto()
        {
            
        }

    }
}
