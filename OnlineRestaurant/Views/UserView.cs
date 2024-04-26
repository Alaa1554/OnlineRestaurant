using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class UserView
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string UserImgUrl { get; set; }
        public string Role { get; set; }
    }
}
