using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class ChefImfService : IImgService<Chef>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> AllowedExtention = new List<string> { ".jpg", ".png" };
        private readonly long AllowedSize = 1048576;


        public ChefImfService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImg(Chef chef)
        {
            if (!string.IsNullOrEmpty(chef.ChefImgUrl))
            {
                var ImgPath=_webHostEnvironment.WebRootPath + chef.ChefImgUrl;
                if(File.Exists(ImgPath))
                {
                    File.Delete(ImgPath);
                }
            }
        }

        public string SetImage(Chef chef, IFormFile? file)
        {
            if (file == null)
            {
                chef.ChefImgUrl = "\\Images\\Noimg.jpg";
                return string.Empty;
            }
            else
            {

                if (!AllowedExtention.Contains(Path.GetExtension(file.FileName).ToLower()))
                    return "This Extention is Not Allowed";
                if (file.Length > AllowedSize)
                    return "The FileSize is Not Allowed";
                var Imgextention = Path.GetExtension(file.FileName).ToLower();
                Guid Imgguid = Guid.NewGuid();
                var createpath = Imgguid.ToString() + Imgextention;
                var ImgUrl = "\\Images\\" + createpath;
                chef.ChefImgUrl = ImgUrl;
                var imgpath = _webHostEnvironment.WebRootPath + ImgUrl;
                using var datafile = new FileStream(imgpath, FileMode.Create);
                file.CopyTo(datafile);
                return string.Empty;
            }
           

        }
        public void UpdateImg(Chef chef, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(chef);
                var message =  SetImage(chef, file);
                if (message != null)
                {
                    chef.Message = message;
                }
            }
        }
    }
}
