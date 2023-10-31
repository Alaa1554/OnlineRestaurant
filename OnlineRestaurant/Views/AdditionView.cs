using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;


namespace OnlineRestaurant.Views
{
    public class AdditionView
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
      
        public List<Choice> Choices { get; set; }
    }
}
