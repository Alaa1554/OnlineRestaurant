using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;


using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class MealView
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Image")]
        public string MealImgUrl { get; set; }
        public string ChefName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? OldPrice { get; set; }
        public int ChefId { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public decimal Rate { get; set; }
        public int NumOfRate { get; set; }
        
    }
}
