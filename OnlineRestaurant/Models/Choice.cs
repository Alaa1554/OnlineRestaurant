using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Choice
    {
        
        public string Name { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; set; }

    }
}
