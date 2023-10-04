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
            _context.SaveChanges();
            return Chef;

           
        }

        public Chef DeleteChefAsync(Chef chef)
        {
            _imgService.DeleteImg(chef);
            _context.Chefs.Remove(chef);
            _context.SaveChanges();
            return chef;
         
        }

        public async Task<Chef> GetChefByIdAsync(int id)
        {
           var chef = await _context.Chefs.SingleOrDefaultAsync(chef => chef.Id==id);
            if (chef == null)
                return new Chef { Message = $"There is no Chef with Id :{id}" };

            return chef;
        }

        public async Task<IEnumerable<ChefView>> GetChefsAsync()
        {
            var chefs =await _context.Chefs.Include(c=>c.ChefReviews).Select(c=>new ChefView { 
                Id=c.Id,
                CategoryId=c.CategoryId,
                ChefImgUrl=c.ChefImgUrl,
                Name=c.Name,
                Rate= decimal.Round(c.ChefReviews.Sum(b => b.Rate) /
               c.ChefReviews.Where(b => b.Rate > 0).DefaultIfEmpty().Count(), 1),
                NumOfRate =c.ChefReviews.Count(c=>c.Rate>0),
            })
                .ToListAsync();
            return chefs;
        }

        public  Chef UpdateChefAsync(Chef chef,UpdateChefDto chefDto)
        {
            var errormessages = ValidateHelper<UpdateChefDto>.Validate(chefDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Chef { Message = errormessages };
            }
            if (!_context.Categories.Any(b => b.Id == chefDto.CategoryId))
            {
                return new Chef { Message = $"No Category with id :{chef.CategoryId} is found" };
            }
            _imgService.UpdateImg(chef, chefDto.ChefImg);
            if (!string.IsNullOrEmpty(chef.Message))
                return new Chef { Message = chef.Message };
                chef.Name = chefDto.Name?? chef.Name ;
            chef.CategoryId = chefDto.CategoryId?? chef.CategoryId;
            _context.Update(chef);
            _context.SaveChanges();
            return chef;
        }
        
    }
}
