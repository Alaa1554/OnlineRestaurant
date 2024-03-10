using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [ValidateNever]
        public string CategoryUrl { get; set; }

        [NotMapped, ValidateNever, JsonIgnore]
        public string Message { get; set; }
    }
}
