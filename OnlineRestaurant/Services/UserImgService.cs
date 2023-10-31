
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

        public string SetImage(ApplicationUser register, IFormFile? file)
        {
            if(file == null)
            {
                register.UserImgUrl=null;
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
                register.UserImgUrl = imgUrl;
                var imgpath = _webHostEnvironment.WebRootPath + imgUrl;
                using var datafile = new FileStream(imgpath, FileMode.Create);
                file.CopyTo(datafile);
                return string.Empty;
            }
                
            
        }
        public void UpdateImg(ApplicationUser register, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(register);
                var message = SetImage(register, file);
                if (message != null)
                {
                    register.Message = message;
                }
            }
        }
    }
}
