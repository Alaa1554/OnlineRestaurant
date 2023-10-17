
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using OnlineRestaurant.Data;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using OnlineRestaurant.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = null;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWishListService, WishListService>();
builder.Services.AddScoped<IChefService, ChefService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IMealReviewService, MealReviewService>();
builder.Services.AddScoped<IChefReviewService, ChefReviewService>();
builder.Services.AddScoped<IMealFilterService, MealFilterService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IMealAdditionService, MealAdditionService>();
builder.Services.AddScoped<IStaticMealAdditionService, StaticMealAdditionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImgService<Chef>, ChefImfService>();
builder.Services.AddScoped<IImgService<ApplicationUser>, UserImgService>();
builder.Services.AddScoped<IImgService<Meal>, MealImgService>();
builder.Services.AddScoped<IImgService<Category>, CategoryimgService>();
builder.Services.AddScoped<IImgService<StaticMealAddition>, AdditionImgService>();
builder.Services.AddCors();

builder.Services.AddTransient<ICartService, CartService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey ( Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});



builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
