using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class AdditionImgService : IImgService<StaticMealAddition>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> AllowedExtention = new List<string> { ".jpg", ".png" };
        private readonly long AllowedSize = 1048576;

        public AdditionImgService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImg(StaticMealAddition addition)
        {
            if (!string.IsNullOrEmpty(addition.AdditionUrl))
            {
                var oldpath = _webHostEnvironment.WebRootPath + addition.AdditionUrl;
                if (File.Exists(oldpath))
                {
                    File.Delete(oldpath);
                }
            }
        }

        public string SetImage(StaticMealAddition Addition, IFormFile? file)
        {
            if (file == null)
            {
                Addition.AdditionUrl = "\\Images\\Noimg.jpg";
                return string.Empty;
            }
            else
            {

                if (!AllowedExtention.Contains(Path.GetExtension(file.FileName).ToLower()))
                    return "This Extention is Not Allowed";
                if (file.Length > AllowedSize)
                    return "The FileSize is Not Allowed";

                var imgextention = Path.GetExtension(file.FileName).ToLower();
                Guid guid = Guid.NewGuid();
                var createpath = guid.ToString() + imgextention;
                var imgUrl = "\\Images\\" + createpath;
                Addition.AdditionUrl = imgUrl;
                var imgpath = _webHostEnvironment.WebRootPath + imgUrl;
                using var datafile = new FileStream(imgpath, FileMode.Create);
                file.CopyTo(datafile);
                return string.Empty;
            }
        }
        public void UpdateImg(StaticMealAddition addition, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(addition);
                var message = SetImage(addition, file);
                if (message != null)
                {
                    addition.Message = message;
                }
            }

        }
    }

}
