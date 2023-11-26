using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class CategoryimgService : IImgService<Category>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> AllowedExtention = new List<string> { ".jpg", ".png" };
        private readonly long AllowedSize = 1048576;

        public CategoryimgService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImg(Category model)
        {
            if(!string.IsNullOrEmpty(model.CategoryUrl))
            {
                var imgpath = _webHostEnvironment.WebRootPath+model.CategoryUrl;
                if(File.Exists(imgpath))
                {
                    File.Delete(imgpath);
                }
            }
        }

        public void SetImage(Category model, IFormFile? file)
        {
            if (file == null)
            {
                model.CategoryUrl = "\\Images\\Noimg.jpg";
                
            }
            else
            {

                if (!AllowedExtention.Contains(Path.GetExtension(file.FileName).ToLower()))
                    model.Message= "This Extention is Not Allowed";
                if (file.Length > AllowedSize)
                    model.Message = "The FileSize is Not Allowed";
                var Imgextention = Path.GetExtension(file.FileName).ToLower();
                Guid Imgguid = Guid.NewGuid();
                var createpath = Imgguid.ToString() + Imgextention;
                var ImgUrl = "\\Images\\" + createpath;
                model.CategoryUrl = "https://localhost:7166" + ImgUrl;
                var imgpath = _webHostEnvironment.WebRootPath + ImgUrl;
                using var datafile = new FileStream(imgpath, FileMode.Create);
                file.CopyTo(datafile);
                
            }
        }

        public void UpdateImg(Category model, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(model);
                 SetImage(model, file);
                
            }
        }
    }
}
