using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class AdditionView
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Choice> Choices { get; set; }
    }
}
