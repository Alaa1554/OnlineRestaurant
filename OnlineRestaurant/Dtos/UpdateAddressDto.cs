
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class UpdateAddressDto
    {
        [MaxLength(100)]
        public string? Street { get; set; }
        [MaxLength(50)]
        public string? City { get; set; }
       
        public int? DepartmentNum { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string? PhoneNumber { get; set; }



    }
}
