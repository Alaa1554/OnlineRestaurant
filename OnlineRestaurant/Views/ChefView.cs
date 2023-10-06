using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
