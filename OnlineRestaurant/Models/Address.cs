using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class Address
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Street { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
       
        public int DepartmentNum { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
        [ValidateNever,JsonIgnore]
        public ApplicationUser User { get; set; }
        [ValidateNever,JsonIgnore]
        public string UserId { get; set; }
        [NotMapped,ValidateNever,JsonIgnore]
        public string Message { get; set; }
    }
}
