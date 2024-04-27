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
            if (!await _context.Categories.AnyAsync(b=>b.Id == chef.CategoryId)) 
                return new ChefDto { Message = $"No Category with id :{chef.CategoryId} is found" };
            
            chef.ChefImgUrl = _imgService.Upload(chef.ChefImg);
            await _context.Chefs.AddAsync(chef);
            await _context.SaveChangesAsync();
            var chefDto=_mapper.Map<ChefDto>(chef);
            return chefDto;

        }

        public async Task<ChefDto> DeleteChefAsync(int id)
        {
            var chef = await _context.Chefs.Include(c => c.Meals).SingleOrDefaultAsync(chef => chef.Id == id);
            if (chef == null)
                return new ChefDto { Message = $"There is no Chef with Id :{id}" };
            if (chef.Meals.Any())
                return new ChefDto { Message = "لا يمكن حذف الشيف الا بعد حذف كل وجباته" };

            _imgService.Delete(chef.ChefImgUrl);
            _context.Chefs.Remove(chef);
            await _context.SaveChangesAsync();
            var chefDto = _mapper.Map<ChefDto>(chef);
            return chefDto;
        }
   
        public async Task<ChefDto> GetChefByIdAsync(int id)
        {
            return _mapper.Map<ChefDto>(await _context.Chefs.SingleOrDefaultAsync(chef => chef.Id==id));
        }

        public IEnumerable<ChefView> GetChefs(PaginateDto dto)
        {
            return _mapper.Map<IEnumerable<ChefView>>(_context.Chefs.Include(c => c.ChefReviews).Include(c => c.Meals).Include(c => c.Category).Paginate(dto.Page, dto.Size));
        }

        public IEnumerable<ChefDto> GetChefsByCategoryIdAsync(int id, PaginateDto dto)
        {
            return _mapper.Map<IEnumerable<ChefDto>>(_context.Chefs.Where(c => c.CategoryId == id).Paginate(dto.Page, dto.Size));
        }
        public async Task<ChefDto> UpdateChefAsync(int id,UpdateChefDto chefDto)
        {
             var chef= await _context.Chefs.Include(c => c.Meals).SingleOrDefaultAsync(chef => chef.Id == id);
            if (chef == null)
                return new ChefDto { Message = $"There is no Chef with Id :{id}" };
            if (!await _context.Categories.AnyAsync(b => b.Id == chefDto.CategoryId))
                    return new ChefDto { Message = $"No Category with id :{chef.CategoryId} is found" };
             if(chef.Meals.Any())
                    chef.Meals.ForEach(m => m.CategoryId = chefDto.CategoryId);

            chef.ChefImgUrl =chefDto.ChefImg==null?chef.ChefImgUrl:_imgService.Update(chef.ChefImgUrl, chefDto.ChefImg);
            _mapper.Map(chefDto,chef);
            await _context.SaveChangesAsync();
            var chefView = _mapper.Map<ChefDto>(chef);
            return chefView;
        }
        
    }
}
