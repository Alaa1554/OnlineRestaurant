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
    public class StaticMealAdditionService : IStaticMealAdditionService
    {
        private readonly IImageService _imgService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StaticMealAdditionService(IImageService imgService, ApplicationDbContext context, IMapper mapper)
        {
            _imgService = imgService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<StaticMealAdditionView> CreateMealAddition(StaticMealAddition Dto)
        {
            var errormessages = ValidateHelper<StaticMealAddition>.Validate(Dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new StaticMealAdditionView { Message = errormessages };
            }
           

            var mealAddition = new StaticMealAddition
            {
                Name = Dto.Name,
                Price = Dto.Price,
            };
             mealAddition.AdditionUrl=_imgService.Upload(Dto.AdditionImg);
             if (!string.IsNullOrEmpty(mealAddition.Message))
                return new StaticMealAdditionView { Message = mealAddition.Message };
            await _context.StaticAdditions.AddAsync(mealAddition);
            await _context.SaveChangesAsync();
            var staticAdditionView = _mapper.Map<StaticMealAdditionView>(mealAddition);
            return staticAdditionView;
        }

        public async Task<StaticMealAdditionView> DeleteMealAddition(StaticMealAddition mealAddition)
        {
            _imgService.Delete(mealAddition.AdditionUrl);
            _context.Remove(mealAddition);
            await _context.SaveChangesAsync();
            var staticAdditionView = _mapper.Map<StaticMealAdditionView>(mealAddition);
            return staticAdditionView;
        }

        public async Task<StaticMealAddition> GetMealAdditionByIdAsync(int id)
        {
            var mealaddition = await _context.StaticAdditions.SingleOrDefaultAsync(addition => addition.Id == id);
            if (mealaddition == null)
                return new StaticMealAddition { Message = $"There is no Addition with Id :{id}" };
            return mealaddition;
        }
       

        public  async Task<StaticMealAdditionView> UpdateMealAdditionAsync(StaticMealAddition mealAddition, UpdateStaticMealAdditionDto dto)
        {
            var errormessages = ValidateHelper<UpdateStaticMealAdditionDto>.Validate(dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new StaticMealAdditionView { Message = errormessages };
            }

            mealAddition.AdditionUrl=dto.AdditionImg==null?mealAddition.AdditionUrl:_imgService.Update(mealAddition.AdditionUrl, dto.AdditionImg);
            if (!string.IsNullOrEmpty(mealAddition.Message))
                return new StaticMealAdditionView { Message = mealAddition.Message };
            mealAddition.Name = dto.Name ?? mealAddition.Name;
            mealAddition.Price = dto.Price ?? mealAddition.Price;
            _context.Update(mealAddition);
            await _context.SaveChangesAsync();
            var staticAdditionView = _mapper.Map<StaticMealAdditionView>(mealAddition);
            return staticAdditionView;
        }

        public IEnumerable<StaticMealAdditionView> GetAllAdditions(PaginateDto dto)
        {
            var Additions = _context.StaticAdditions.Paginate(dto.Page, dto.Size).ToList();
            var staticAdditionsView = _mapper.Map<IEnumerable<StaticMealAdditionView>>(Additions);
            return staticAdditionsView;
        }
    }
}
