using OnlineRestaurant.Interfaces;

namespace OnlineRestaurant.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void Delete(string imageName)
        {
            if(imageName!= "Noimg.jpg")
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Images", imageName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            
        }

        public string Update(string imageName, IFormFile file)
        {
            Delete(imageName);
            return Upload(file);
        }

        public string Upload(IFormFile image)
        {
            if (image == null)
                return "Noimg.jpg";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
            var ImageName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var ImagePath = Path.Combine(uploadPath, ImageName);
            using var fileStream = new FileStream(ImagePath, FileMode.Create);
            image.CopyTo(fileStream);
            return ImageName;
        }
    }
}
