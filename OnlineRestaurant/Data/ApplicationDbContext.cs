using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineRestaurant.Models;
using System.Text.Json.Serialization;

namespace OnlineRestaurant.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
      
        public DbSet<Chef> Chefs { get; set; }
        
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MealAddition> MealAdditions { get; set; }
        public DbSet<MealReview> MealReviews { get; set; }
        public DbSet<ChefReview> ChefReviews { get; set; }
      
        
    }
}
