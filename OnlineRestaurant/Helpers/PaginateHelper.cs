using OnlineRestaurant.Views;

namespace OnlineRestaurant.Helpers
{
    public static class PaginateHelper
    {
        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int page, int size)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (size <= 0)
            {
                size = 10;
            }

            var result = source.Skip((page - 1) * size).Take(size).ToList();

            return result;
        }
    }
}
