
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class MealImgService : IImgService<Meal>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> AllowedExtention = new List<string> { ".jpg", ".png" };
        private readonly long AllowedSize = 1048576;

        public MealImgService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImg(Meal meal)
        {
            if (!string.IsNullOrEmpty(meal.MealImgUrl)) 
            {
                var oldpath =_webHostEnvironment.WebRootPath+meal.MealImgUrl;
                if(File.Exists(oldpath))
                {
                    File.Delete(oldpath);
                }
            }
        }

        public void SetImage(Meal meal, IFormFile? file)
        {
            if (file == null)
            {
                meal.MealImgUrl = "\\Images\\Noimg.jpg";
            }
            else
            {
                
                if (!AllowedExtention.Contains(Path.GetExtension(file.FileName).ToLower()))
                    meal.Message= "This Extention is Not Allowed";
                if (file.Length > AllowedSize)
                   meal.Message= "The FileSize is Not Allowed";

                var imgextention=Path.GetExtension(file.FileName).ToLower();
                Guid guid = Guid.NewGuid();
                var createpath = guid.ToString() + imgextention;
                var imgUrl = "\\Images\\" + createpath;
                meal.MealImgUrl = "https://localhost:7166" + imgUrl;
                var imgpath=_webHostEnvironment.WebRootPath+imgUrl;
                using var datafile =new FileStream(imgpath,FileMode.Create);
                file.CopyTo(datafile);
            }
        }
        public void UpdateImg(Meal meal, IFormFile? file)
        {
            if (file != null)
            {
                DeleteImg(meal);
                SetImage(meal, file);
               
            }
           
        }
    }
}
