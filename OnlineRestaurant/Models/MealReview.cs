using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class MealReview
    {
        public int Id { get; set; }
        [Required,MaxLength(100)]
        public string Text  { get; set; }
        [ValidateNever,JsonIgnore]
        public string UserId { get; set; }
        [ValidateNever]
        public string UserName { get; set; }
        [ValidateNever]
        public DateTime CreatedDate { get; set; }
        [ValidateNever]
        public string? UserImg { get; set; } 
        [ValidateNever,JsonIgnore]
        public Meal Meal { get; set; }
        public int MealId { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Rate must be more than -1")]
        public decimal Rate { get; set; }
        
        [NotMapped,ValidateNever,JsonIgnore]
        public string Message { get; set; }
    }
}
