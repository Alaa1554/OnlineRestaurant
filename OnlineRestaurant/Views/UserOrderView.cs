using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Views
{
    public class UserOrderView
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }

        public string Status { get; set; }

        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        [MaxLength(100)]
        public string Street { get; set; }
        [MaxLength(50)]
        public string City { get; set; }

        public int DepartmentNum { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
        public int NumOfMeals { get; set; }
        public int NumOfStaticMealAdditions { get; set; }
    }
}
