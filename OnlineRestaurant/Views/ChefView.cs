
using System.ComponentModel;

namespace OnlineRestaurant.Views
{
    public class ChefView
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        [DisplayName("Image")]
        public string ChefImgUrl { get; set; }
       
        public string CategoryName { get; set; }
        public decimal Rate { get; set; }
        public int NumOfRate { get; set; }
        public int NumOfMeals { get; set; }


    }
}
