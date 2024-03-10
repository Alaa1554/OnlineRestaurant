using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class StaticMealAdditionView
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price must be more than -1")]
        public decimal Price { get; set; }
        [ValidateNever]
        public string AdditionUrl { get; set; }

        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}
