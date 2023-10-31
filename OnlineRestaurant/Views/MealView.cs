
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Views
{
    public class MealView
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Image")]
        public string MealImgUrl { get; set; }
        public string ChefName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? OldPrice { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string? IsFavourite { get; set; }
        public int ChefId { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public decimal Rate { get; set; }
        public int NumOfRate { get; set; }
        
        public override bool Equals(object? obj)
        {
            if(obj == null&&!(obj is MealView)) 
                return false;
            var mealview = obj as MealView;
            return this.Id==mealview.Id;
        }
        public override int GetHashCode()
        {
            int hash = 13;
            hash=(hash*7)+Id.GetHashCode();
            hash=(hash*7)+ Name.GetHashCode();
            hash=(hash*7)+ Price.GetHashCode();
            hash=(hash*7)+ ChefId.GetHashCode();
            return hash;
        }
    }
}
