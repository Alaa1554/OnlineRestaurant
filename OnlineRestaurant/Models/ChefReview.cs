using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class ChefReview
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Text { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        [ValidateNever]
        public string UserName { get; set; }
        [ValidateNever]
        public DateTime CreatedDate { get; set; }
        [ValidateNever, JsonIgnore]
        public Chef Chef { get; set; }
        public int ChefId { get; set; }
        [Required]
        public decimal Rate { get; set; }
        [Required,NotMapped]
        public TokenModel TokenModel { get; set; }

        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}
