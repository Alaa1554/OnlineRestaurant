

namespace OnlineRestaurant.Interfaces
{
    public interface IImageService
    {
        string Upload(IFormFile image);
        void Delete(string imageName);
        string Update(string imageName, IFormFile file);
    }
}
