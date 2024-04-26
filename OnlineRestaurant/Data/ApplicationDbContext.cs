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
            builder.Entity<WishList>().HasMany(c => c.Meals).WithMany(c => c.WishLists).UsingEntity<WishListMeal>();
            builder.Entity<Meal>()
            .HasIndex(e => e.Name)
            .IsUnique();
            builder.Entity<Order>().HasMany(o=>o.StaticMealAdditions).WithMany(s=>s.Orders).UsingEntity<OrderStaticAddition>();
            builder.Entity<Order>().HasMany(o=>o.Meals).WithMany(m=>m.Orders).UsingEntity<OrderMeal>();
            builder.Entity<OrderMeal>()
                .HasKey(c => new { c.OrderId, c.MealId, c.Addition });
        }
        public DbSet<Chef> Chefs { get; set; }
        
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MealAddition> MealAdditions { get; set; }
        public DbSet<MealReview> MealReviews { get; set; }
        public DbSet<ChefReview> ChefReviews { get; set; }
        public DbSet<StaticMealAddition> StaticAdditions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WishListMeal> WishListMeals { get;set; }
        public DbSet<WishList> wishLists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStaticAddition> OrdersStaticAdditions { get;set; }
        public DbSet<OrderMeal> OrderMeals { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Choice> Choices { get; set; }
    }
}
