
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; }
        [Required,MaxLength(50)]
        public string LastName { get; set; }
        public string? UserImgUrl { get; set; }
        [ValidateNever,NotMapped,JsonIgnore]
        public string Message { get; set; }
        [ValidateNever, JsonIgnore]
        public List<Address> Addresses { get; set; }
        [ValidateNever, JsonIgnore]
        public  WishList WishList { get; set; }
        [ValidateNever, JsonIgnore]
        public List<Order> Orders { get; set; }

    }
}
