

namespace OnlineRestaurant.Interfaces
{
    public interface IImgService<T> where T : class
    {
        void SetImage(T model, IFormFile? file);
        void DeleteImg(T model);
        void UpdateImg(T model, IFormFile? file);
    }
}
