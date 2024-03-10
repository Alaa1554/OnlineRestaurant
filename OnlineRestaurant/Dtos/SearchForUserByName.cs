using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Dtos
{
    public class SearchForUserByName
    {
        public string UserName { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue)]
        public int Size { get; set; } = 10;
    }
}
