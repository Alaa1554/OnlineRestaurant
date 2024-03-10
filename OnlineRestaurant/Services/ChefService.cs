using AutoMapper;
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
       
        private readonly IImageService _imgService;
        private readonly IMapper _mapper;

        public ChefService(ApplicationDbContext context, IImageService imgService, IMapper mapper)
        {
            _context = context;
            _imgService = imgService;
            _mapper = mapper;
        }
        public async Task<ChefDto> CreateChef(Chef chef)
        {
            var errormessages = ValidateHelper<Chef>.Validate(chef);
            if (!string.IsNullOrEmpty(errormessages)) 
            {
                return new ChefDto { Message = errormessages };
            }
            if (!await _context.Categories.AnyAsync(b=>b.Id == chef.CategoryId)) 
            {
                return new ChefDto { Message = $"No Category with id :{chef.CategoryId} is found" };
            }
            var Chef = new Chef
            {
                Name = chef.Name,
                CategoryId = chef.CategoryId,
            };
            Chef.ChefImgUrl = _imgService.Upload(chef.ChefImg);
            if (!string.IsNullOrEmpty(Chef.Message)) 
                return new ChefDto { Message = Chef.Message };
            await _context.Chefs.AddAsync(Chef);
            await _context.SaveChangesAsync();
            var chefDto=_mapper.Map<ChefDto>(Chef);
            return chefDto;

           
        }

        public async Task<ChefDto> DeleteChefAsync(Chef chef)
        {
            if(await _context.Meals.AnyAsync(c => c.ChefId == chef.Id))
            {
                return new ChefDto { Message = "لا يمكن حذف الشيف الا بعد حذف كل وجباته" };
            }
            _imgService.Delete(chef.ChefImgUrl);
            _context.Chefs.Remove(chef);
           await _context.SaveChangesAsync();
            var chefDto = _mapper.Map<ChefDto>(chef);
            return chefDto;

        }

        public async Task<Chef> GetChefByIdAsync(int id)
        {
           var chef = await _context.Chefs.Include(c=>c.Category).SingleOrDefaultAsync(chef => chef.Id==id);
            if (chef == null)
                return new Chef { Message = $"There is no Chef with Id :{id}" };
            chef.CategoryName = chef.Category.Name;
            return chef;
        }

        public IEnumerable<ChefView> GetChefs(PaginateDto dto)
        {
            var chefs = _context.Chefs.Include(c=>c.ChefReviews).Include(c=>c.Meals).Include(c=>c.Category).Paginate(dto.Page, dto.Size).Select(c=>new ChefView { 
                Id=c.Id,
                CategoryName=c.Category.Name,
                ChefImgUrl=Path.Combine("https://localhost:7166","images", c.ChefImgUrl),
                Name=c.Name,
                Rate= decimal.Round(c.ChefReviews.Sum(b => b.Rate) /
               c.ChefReviews.Where(b => b.Rate > 0).DefaultIfEmpty().Count(), 1),
                NumOfRate =c.ChefReviews.Count(c=>c.Rate>0),
                NumOfMeals=c.Meals.Count()
            }).ToList();
            
            return chefs;
        }

        public async Task<IEnumerable<ChefDto>> GetChefsByCategoryIdAsync(int id, PaginateDto dto)
        {
            var chefs = await _context.Chefs.Where(c=>c.CategoryId==id).ToListAsync();
            var chefsDto=_mapper.Map<IEnumerable<ChefDto>>(chefs);
            var result = chefsDto.Paginate(dto.Page, dto.Size);
            return result;
        }
        public async Task<ChefDto> UpdateChefAsync(Chef chef,UpdateChefDto chefDto)
        {
            var errormessages = ValidateHelper<UpdateChefDto>.Validate(chefDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new ChefDto { Message = errormessages };
            }
            if (chefDto.CategoryId.HasValue)
            {
                if (!await _context.Categories.AnyAsync(b => b.Id == chefDto.CategoryId))
                {
                    return new ChefDto { Message = $"No Category with id :{chef.CategoryId} is found" };
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


            chef.ChefImgUrl = chefDto.ChefImg == null ? chef.ChefImgUrl : _imgService.Update(chef.ChefImgUrl, chefDto.ChefImg);
                if (!string.IsNullOrEmpty(chef.Message))
                    return new ChefDto { Message = chef.Message };
            
           
                chef.Name = chefDto.Name?? chef.Name ;
               
            _context.Update(chef);
            await _context.SaveChangesAsync();
            var chefView = _mapper.Map<ChefDto>(chef);
            return chefView;
        }
        
    }
}
