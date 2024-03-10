using AutoMapper;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModelDto, ApplicationUser>().ForMember(c=>c.UserImgUrl,ca=>ca.Ignore());
            CreateMap<CouponDto, Coupon>().
                ForMember(c => c.CouponCode, m => m.MapFrom(i => i.CouponCode.Trim())) ;
            CreateMap<GmailRegisterDto, ApplicationUser>();
            CreateMap<MealReview, MealReviewView>().ForMember(c => c.UserImg, m => m.MapFrom(i =>
                 i.UserImg == null ? null : Path.Combine("https://localhost:7166", "images", i.UserImg))); ;
            CreateMap<ChefReview, ChefReviewView>()
                .ForMember(c => c.UserImg, m => m.MapFrom(i =>
                 i.UserImg==null?null:Path.Combine("https://localhost:7166", "images", i.UserImg)));
            CreateMap<Category,CategoryDto>().
                ForMember(c=>c.CategoryUrl,m=>m.MapFrom(i=> Path.Combine("https://localhost:7166","images", i.CategoryUrl)));
            CreateMap<Chef, ChefDto>().
                ForMember(c => c.ChefImgUrl, m => m.MapFrom(i => Path.Combine("https://localhost:7166", "images", i.ChefImgUrl)));
            CreateMap<Meal, MealDto>().
                ForMember(c => c.MealImgUrl, m => m.MapFrom(i => Path.Combine("https://localhost:7166", "images", i.MealImgUrl)));
            CreateMap<StaticMealAddition, StaticMealAdditionView>().
                ForMember(c => c.AdditionUrl, m => m.MapFrom(i => Path.Combine("https://localhost:7166", "images", i.AdditionUrl)));
        }
    }
}
