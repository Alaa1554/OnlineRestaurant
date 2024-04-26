using AutoMapper;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;
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
            CreateMap<InsertMealDto, Meal>().
                ForMember(des => des.MealImgUrl, m => m.Ignore()).
                ForMember(des=>des.Description,m=>m.MapFrom(src=>src.Description==null?null:src.Description)).
                ForMember(des=>des.OldPrice,m=>m.MapFrom(src=>src.OldPrice==null?null:src.OldPrice));
            CreateMap<Meal, MealDto>().
                ForMember(c => c.MealImgUrl, m => m.MapFrom(i => Path.Combine("https://localhost:7166", "images", i.MealImgUrl)));
            CreateMap<StaticMealAddition, StaticMealAdditionView>().
                ForMember(c => c.AdditionUrl, m => m.MapFrom(i => Path.Combine("https://localhost:7166", "images", i.AdditionUrl)));
            CreateMap<UpdateAddressDto, Address>();
            CreateMap<MealAddition, AdditionView>();
            CreateMap<UpdateMealAdditionDto, MealAddition>();
            CreateMap<UpdateReviewDto, MealReview>();
            CreateMap<UpdateStaticMealAdditionDto, StaticMealAddition>().ForMember(des=>des.AdditionUrl,m=>m.Ignore());
            CreateMap<UpdateChefDto, Chef>().ForMember(des=>des.ChefImg,m=>m.Ignore());
            CreateMap<Chef, ChefView>().
                ForMember(des=>des.ChefImgUrl,m=>m.MapFrom(src=> Path.Combine("https://localhost:7166", "images", src.ChefImgUrl))).
                ForMember(des=>des.CategoryName,m=>m.MapFrom(src=>src.Category.Name)).
                ForMember(des=>des.NumOfMeals,m=>m.MapFrom(src=>src.Meals.Count())).
                ForMember(des=>des.NumOfRate,m=>m.MapFrom(src=>src.ChefReviews.Count())).
                ForMember(des=>des.Rate,m=>m.MapFrom(src=> decimal.Round(src.ChefReviews.Sum(cr => cr.Rate) / src.ChefReviews.DefaultIfEmpty().Count(), 1)));
            CreateMap<Category, CategoryView>().
                  ForMember(c => c.CategoryImg, m => m.MapFrom(src => Path.Combine("https://localhost:7166", "images", src.CategoryUrl))).
                  ForMember(des => des.NumOfMeals, m => m.MapFrom(src => src.Meals.Count())).
                  ForMember(des => des.NumOfChefs, m => m.MapFrom(src => src.Chefs.Count()));
            CreateMap<MealReviewDto, MealReview>().
                ForMember(des => des.Id,m=>m.MapFrom(src=>src.MealReview.Id)).
                ForMember(des => des.UserName, m => m.MapFrom(src => src.User.UserName)).
                ForMember(des => des.CreatedDate, m => m.MapFrom(src => DateTime.Now)).
                ForMember(des => des.UserId, m => m.MapFrom(src => src.UserId)).
                ForMember(des => des.MealId, m => m.MapFrom(src => src.MealReview.MealId)).
                ForMember(des => des.Rate, m => m.MapFrom(src => src.MealReview.Rate)).
                ForMember(des => des.Text, m => m.MapFrom(src => src.MealReview.Text)).
                ForMember(des => des.UserImg, m => m.MapFrom(src => src.User.UserImgUrl));
            CreateMap<MapMealDto, MealByNameView>().
                ForMember(des => des.Name, m => m.MapFrom(src => src.Meal.Name)).
                ForMember(des => des.Id, m => m.MapFrom(src => src.Meal.Id)).
                ForMember(des => des.CategoryName, m => m.MapFrom(src => src.Meal.Category.Name)).
                ForMember(des => des.ChefName, m => m.MapFrom(src => src.Meal.Chef.Name)).
                ForMember(des => des.Description, m => m.MapFrom(src => src.Meal.Description)).
                ForMember(des => des.Image, m => m.MapFrom(src => Path.Combine("https://localhost:7166", "images", src.Meal.MealImgUrl))).
                ForMember(des => des.Price, m => m.MapFrom(src => src.Meal.Price)).
                ForMember(des => des.OldPrice, m => m.MapFrom(src => src.Meal.OldPrice)).
                ForMember(des => des.Rate, m => m.MapFrom(src => decimal.Round(src.Meal.MealReviews.Sum(cr => cr.Rate) / src.Meal.MealReviews.DefaultIfEmpty().Count(), 1))).
                ForMember(des => des.NumOfRates, m => m.MapFrom(src => src.Meal.MealReviews.Count())).
                ForMember(des => des.StaticMealAdditions, m => m.MapFrom(src => src.StaticMealAdditions)).
                ForMember(des => des.MealAdditions, m => m.MapFrom(src => src.Meal.Additions)).
                ForMember(des => des.Reviews, m => m.MapFrom(src => src.Meal.MealReviews)).
                ForMember(des => des.ChefId, m => m.MapFrom(src => src.Meal.ChefId)).
                ForMember(des => des.CategoryId, m => m.MapFrom(src => src.Meal.CategoryId)).
                ForMember(des => des.IsFavourite, m => m.MapFrom(src => src.IsFavourite));
            CreateMap<InsertOrderDto, Order>().
                ForMember(des => des.Id, m => m.MapFrom(src => Guid.NewGuid().ToString())).
                ForMember(des => des.Status, m => m.MapFrom(src => "Processing")).
                ForMember(des => des.StatusDate, m => m.MapFrom(src => DateTime.Now)).
                ForMember(des => des.Date, m => m.MapFrom(src => DateTime.Now)).
                ForMember(des => des.PaymentMethod, m => m.MapFrom(src => src.OrderDto.PaymentMethod)).
                ForMember(des => des.UserId, m => m.MapFrom(src => src.UserId)).
                ForMember(des => des.DepartmentNum, m => m.MapFrom(src => src.Address.DepartmentNum)).
                ForMember(des => des.City, m => m.MapFrom(src => src.Address.City)).
                ForMember(des => des.PhoneNumber, m => m.MapFrom(src => src.Address.PhoneNumber)).
                ForMember(des => des.Street, m => m.MapFrom(src => src.Address.Street)).
                ForMember(des => des.TotalCost, m => m.MapFrom(src => src.OrderDto.TotalPrice)).
                ForMember(des => des.NumOfMeals, m => m.MapFrom(src => src.OrderDto.MealOrders.Count())).
                ForMember(des => des.NumOfStaticMealAdditions, m => m.MapFrom(src => src.OrderDto.StaticAdditionOrders.Count())).
                ForMember(des => des.IsPaid, m => m.MapFrom(src => src.OrderDto.PaymentMethod.ToLower().Trim()== "credit"?true:false));
            CreateMap<OrderMeal, OrderMealView>().
                ForMember(des => des.MealName, m => m.MapFrom(src => src.Meal.Name)).
                ForMember(des => des.MealPrice, m => m.MapFrom(src => src.Meal.Price)).
                ForMember(des => des.MealImgUrl, m => m.MapFrom(src => Path.Combine("https://localhost:7166", "images", src.Meal.MealImgUrl)));
            CreateMap<OrderStaticAddition, OrderStaticAdditionView>().
                ForMember(des => des.StaticAdditionName, m => m.MapFrom(src => src.StaticMealAddition.Name)).
                ForMember(des => des.StaticAdditionPrice, m => m.MapFrom(src => src.StaticMealAddition.Price)).
                ForMember(des => des.StaticAdditionImgUrl, m => m.MapFrom(src => Path.Combine("https://localhost:7166", "images", src.StaticMealAddition.AdditionUrl)));
            CreateMap<Order, OrderView>();
            CreateMap<WishListMeal, MealView>().IncludeMembers(src=>src.Meals);
            CreateMap<WishListMealDto, WishListMealView>();
            CreateMap<Order,UserOrderView>();
            CreateMap<Order, AdminOrderView>().
                ForMember(des => des.UserImg, m => m.MapFrom(src => src.User.UserImgUrl == null ? null : Path.Combine("https://localhost:7166", "images", src.User.UserImgUrl))).
                ForMember(des => des.UserName, m => m.MapFrom(src => src.User.UserName));
            CreateMap<Meal, MealView>().
                ForMember(des => des.CategoryName, m => m.MapFrom(src => src.Category.Name)).
                ForMember(des => des.ChefName, m => m.MapFrom(src => src.Chef.Name)).
                ForMember(des => des.Rate, m => m.MapFrom(src => decimal.Round(src.MealReviews.Sum(cr => cr.Rate) / src.MealReviews.DefaultIfEmpty().Count(), 1))).
                ForMember(des => des.NumOfRate, m => m.MapFrom(src => src.MealReviews.Count())).
                ForMember(des => des.MealImgUrl, m => m.MapFrom(src => Path.Combine("https://localhost:7166", "images", src.MealImgUrl)));
        }
    }
}
