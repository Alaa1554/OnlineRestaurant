
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class ChefReviewView
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Text { get; set; }
        
        public string UserImg { get; set; }
        
        public string UserName { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public int ChefId { get; set; }
        
        public decimal Rate { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
    }
}
