using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Dtos
{
    public class ChefDto
    {
        public int Id { get; set; }
        [MaxLength(100)]

        public string Name { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string ChefImgUrl { get; set; }
        [ValidateNever, JsonIgnore]
        public int CategoryId { get; set; }
        
        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
       
    }
}
