
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class UserImgService:IImgService<ApplicationUser>
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> AllowedExtention = new List<string> { ".jpg", ".png" };
        private readonly long AllowedSize = 1048576;

        public UserImgService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImg(ApplicationUser register)
        {
            if (!string.IsNullOrEmpty(register.UserImgUrl))
            {
                var oldpath = _webHostEnvironment.WebRootPath + register.UserImgUrl;
                if (File.Exists(oldpath))
                {
                    File.Delete(oldpath);
                }
            }
        }

        public void SetImage(ApplicationUser register, IFormFile? file)
        {
            if(file == null)
            {
                register.UserImgUrl=null;
                
            }
            else
            {
                if (!AllowedExtention.Contains(Path.GetExtension(file.FileName).ToLower()))
                    register.Message= "This Extention is Not Allowed";
                if (file.Length > AllowedSize)
                    register.Message= "The FileSize is Not Allowed";

                var imgextention = Path.GetExtension(file.FileName).ToLower();
                Guid guid = Guid.NewGuid();
                var createpath = guid.ToString() + imgextention;
                var imgUrl = "\\Images\\" + createpath;
                register.UserImgUrl = "https://localhost:7166" + imgUrl;
                var imgpath = _webHostEnvironment.WebRootPath + imgUrl;
                using var datafile = new FileStream(imgpath, FileMode.Create);
                file.CopyTo(datafile);
            }
                
            
        }
        public void UpdateImg(ApplicationUser register, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(register);
                SetImage(register, file);
            }
        }
    }
}
