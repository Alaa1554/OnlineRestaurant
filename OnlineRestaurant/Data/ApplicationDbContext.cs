using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<WishList>().HasMany(c => c.Meals).WithMany(c => c.WishLists).UsingEntity<WishListMeals>();
        }
        public DbSet<Chef> Chefs { get; set; }
        
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MealAddition> MealAdditions { get; set; }
        public DbSet<MealReview> MealReviews { get; set; }
        public DbSet<ChefReview> ChefReviews { get; set; }
        public DbSet<StaticMealAddition> StaticAdditions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WishListMeals> WishListMeals { get;set; }
        public DbSet<WishList> wishLists { get; set; }
    }
}
