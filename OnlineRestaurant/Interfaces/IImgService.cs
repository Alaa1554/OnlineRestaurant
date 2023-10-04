using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IImgService<T> where T : class
    {
        string SetImage(T model, IFormFile? file);
        void DeleteImg(T model);
        void UpdateImg(T model, IFormFile? file);
    }
}
