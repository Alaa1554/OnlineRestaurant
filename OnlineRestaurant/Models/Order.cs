using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Models
{
    public class Order
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        
        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [MaxLength(100)]
        public string Street { get; set; }
        [MaxLength(50)]
        public string City { get; set; }

        public int DepartmentNum { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
       
        public List<Meal> Meals { get; set; }
        public List<StaticMealAddition> StaticMealAdditions { get; set;}
        public List<OrderStaticAddition> OrderStaticAdditions { get; set;}
        public List<OrderMeal> OrderMeals { get; set; }
    }
}
