using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class ChefService : IChefService
    {
        private readonly ApplicationDbContext _context;
       
        private readonly IImgService<Chef> _imgService;

        public ChefService(ApplicationDbContext context, IImgService<Chef> imgService)
        {
            _context = context;
            _imgService = imgService;
        }
        public async Task<Chef> CreateChef(Chef chef)
        {
            var errormessages = ValidateHelper<Chef>.Validate(chef);
            if (!string.IsNullOrEmpty(errormessages)) 
            {
                return new Chef { Message = errormessages };
            }
            if (!await _context.Categories.AnyAsync(b=>b.Id == chef.CategoryId)) 
            {
                return new Chef { Message = $"No Category with id :{chef.CategoryId} is found" };
            }
            var Chef = new Chef
            {
                Name = chef.Name,
                CategoryId = chef.CategoryId,
            };
           _imgService.SetImage(Chef, chef.ChefImg);
            if (!string.IsNullOrEmpty(Chef.Message)) 
                return new Chef { Message = Chef.Message };
            await _context.Chefs.AddAsync(Chef);
            await _context.SaveChangesAsync();
            return Chef;

           
        }

        public async Task<Chef> DeleteChefAsync(Chef chef)
        {
            if(await _context.Meals.AnyAsync(c => c.ChefId == chef.Id))
            {
                return new Chef { Message = "لا يمكن حذف الشيف الا بعد حذف كل وجباته" };
            }
            _imgService.DeleteImg(chef);
            _context.Chefs.Remove(chef);
           await _context.SaveChangesAsync();
            return chef;
         
        }

        public async Task<Chef> GetChefByIdAsync(int id)
        {
           var chef = await _context.Chefs.Include(c=>c.Category).SingleOrDefaultAsync(chef => chef.Id==id);
            if (chef == null)
                return new Chef { Message = $"There is no Chef with Id :{id}" };
            chef.CategoryName = chef.Category.Name;
            return chef;
        }

        public async Task<IEnumerable<ChefView>> GetChefsAsync()
        {
            var chefs =await _context.Chefs.Include(c=>c.ChefReviews).Include(c=>c.Meals).Include(c=>c.Category).Select(c=>new ChefView { 
                Id=c.Id,
                CategoryName=c.Category.Name,
                ChefImgUrl=c.ChefImgUrl,
                Name=c.Name,
                Rate= decimal.Round(c.ChefReviews.Sum(b => b.Rate) /
               c.ChefReviews.Where(b => b.Rate > 0).DefaultIfEmpty().Count(), 1),
                NumOfRate =c.ChefReviews.Count(c=>c.Rate>0),
                NumOfMeals=c.Meals.Count()
            })
                .ToListAsync();
            return chefs;
        }

        public async Task<IEnumerable<Chef>> GetChefsByCategoryIdAsync(int id)
        {
            var chefs = await _context.Chefs.Where(c=>c.CategoryId==id).ToListAsync();
            return chefs;
        }
        public async Task<Chef> UpdateChefAsync(Chef chef,UpdateChefDto chefDto)
        {
            var errormessages = ValidateHelper<UpdateChefDto>.Validate(chefDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Chef { Message = errormessages };
            }
            if (chefDto.CategoryId.HasValue)
            {
                if (!await _context.Categories.AnyAsync(b => b.Id == chefDto.CategoryId))
                {
                    return new Chef { Message = $"No Category with id :{chef.CategoryId} is found" };
                }
                chef.CategoryId = chefDto.CategoryId??chef.CategoryId;
                if(await _context.Meals.AnyAsync(m => m.ChefId == chef.Id))
                {
                    var meals = _context.Meals.Where(m => m.ChefId == chef.Id);
                    foreach(var meal in meals)
                    {
                        meal.CategoryId = (int)chefDto.CategoryId;
                    }
                    
                }
                chef.CategoryName = null;
            }
           
            
                _imgService.UpdateImg(chef, chefDto.ChefImg);
                if (!string.IsNullOrEmpty(chef.Message))
                    return new Chef { Message = chef.Message };
            
           
                chef.Name = chefDto.Name?? chef.Name ;
               
            _context.Update(chef);
            await _context.SaveChangesAsync();
            return chef;
        }
        
    }
}
