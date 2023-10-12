﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class StaticMealAddition
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public decimal Price { get; set; }
        [ValidateNever]
        public string AdditionUrl { get; set; }
        [NotMapped, Required(ErrorMessage = "This Field is Required"), JsonIgnore]
        public IFormFile AdditionImg { get; set; }
       
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}