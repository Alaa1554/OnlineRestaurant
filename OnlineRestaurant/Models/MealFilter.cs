﻿using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Models
{
    public class MealFilter
    {
        public string? Category { get; set; }
        public string? MealName { get; set; }
        public string? ChefName { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "FromPrice must be more than -1")]
        public int? FromPrice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "ToPrice must be more than -1")]
        public int? ToPrice { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string? OrderMeal { get; set; }
        
     
    }
}
