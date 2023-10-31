using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Chef
    {
        public int Id { get; set; }
        [MaxLength(100)]
        
        public string Name { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string ChefImgUrl { get; set; }
        [ValidateNever,JsonIgnore]
        public Category Category { get; set; }
        [ValidateNever,JsonIgnore]
        public List<Meal> Meals { get; set; }
        public int CategoryId {get; set; }
       
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
        [NotMapped,Required(ErrorMessage = "This Field is Required"),JsonIgnore]
        public IFormFile ChefImg { get; set; }
        [ValidateNever, JsonIgnore]
        public List<ChefReview> ChefReviews { get; set;}
        


    }
}
