using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [ValidateNever]
        public string CategoryUrl { get; set; }
        [ValidateNever,JsonIgnore]
        public List<Meal> Meals { get; set; }
        [ValidateNever,JsonIgnore]
        public List<Chef> Chefs { get; set; }
        [NotMapped,JsonIgnore]
        public IFormFile? CategoryImg { get; set; }
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
       
    }
}
