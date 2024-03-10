using OnlineRestaurant.Views;

namespace OnlineRestaurant.Helpers
{
    public static class PaginateHelper
    {
        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int page, int size)
        {
            
            var result = source.Skip((page - 1) * size).Take(size+1);

            return result;
        }
    }
}
