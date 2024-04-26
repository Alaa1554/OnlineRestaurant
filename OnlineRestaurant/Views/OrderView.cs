using OnlineRestaurant.Migrations;
using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class OrderView
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        
        public DateTime StatusDate { get; set; }
        public string Status { get;set; }

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

        public List<OrderMealView> OrderMeals { get; set; }
        
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public List<OrderStaticAdditionView>? OrderStaticAdditions { get; set; }
        [JsonIgnore]
        public string Message { get; set; }

    }
}
