using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Dtos
{
    public class MealDto
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "This Field is Required")]

        public string Name { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be more than -1")]
        public decimal Price { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string MealImgUrl { get; set; }
        [MaxLength(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
       
        [Required(ErrorMessage = "This Field is Required")]
        public int ChefId { get; set; }
        
        public int CategoryId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Range(0, int.MaxValue, ErrorMessage = "OldPrice must be more than -1")]
        public decimal? OldPrice { get; set; }
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
       
        [ValidateNever]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal Rate { get; set; }
        [ValidateNever]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Range(0, int.MaxValue, ErrorMessage = "NumOfRate can't be less than zero")]
        public int NumOfRate { get; set; }
    }
}
