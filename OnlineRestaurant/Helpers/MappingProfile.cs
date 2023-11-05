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
            CreateMap<GmailRegisterDto, ApplicationUser>();
            CreateMap<MealReview, MealReviewView>();
            CreateMap<ChefReview, ChefReviewView>();

        }
    }
}
