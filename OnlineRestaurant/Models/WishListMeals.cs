﻿using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class WishListMeals
    {
        public int MealId { get; set; }
        public int WishListId { get; set;}
        public WishList WishList { get; set; }
        public Meal Meals { get; set; }
    }
}
