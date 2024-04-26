using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Meal
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage ="This Field is Required")]
       
        public string Name { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        [Range(0, int.MaxValue,ErrorMessage ="Price must be more than -1")]
        public decimal Price { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string MealImgUrl { get; set; }
        [MaxLength(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        [ValidateNever,JsonIgnore]
        public Chef Chef { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public int ChefId { get; set; }
        [ValidateNever,JsonIgnore]
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Range(0, int.MaxValue, ErrorMessage = "OldPrice must be more than -1")]
        public decimal? OldPrice { get; set; }
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
        
        [ValidateNever,JsonIgnore]
        public List<MealAddition> Additions { get; set; }
        [ValidateNever, JsonIgnore]
        public List<MealReview> MealReviews { get; set; }
        [ValidateNever, JsonIgnore]

        public List<WishList> WishLists { get; set; }
        [ValidateNever, JsonIgnore]

        public IEnumerable<WishListMeal> WishListMeal { get; set; }
        [ValidateNever, JsonIgnore]
        public List<Order> Orders { get; set; }
        [ValidateNever, JsonIgnore]
        public List<OrderMeal> OrderMeals { get; set; }

    }
}
