using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class StaticMealAdditionService : IStaticMealAdditionService
    {
        private readonly IImgService<StaticMealAddition> _imgService;
        private readonly ApplicationDbContext _context;

        public StaticMealAdditionService(IImgService<StaticMealAddition> imgService, ApplicationDbContext context)
        {
            _imgService = imgService;
            _context = context;
        }

        public async Task<StaticMealAddition> CreateMealAddition(StaticMealAddition Dto)
        {
            var errormessages = ValidateHelper<StaticMealAddition>.Validate(Dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new StaticMealAddition { Message = errormessages };
            }
           

            var mealAddition = new StaticMealAddition
            {
                Name = Dto.Name,
                Price = Dto.Price,
            };
            _imgService.SetImage(mealAddition, Dto.AdditionImg);
             if (!string.IsNullOrEmpty(mealAddition.Message))
                return new StaticMealAddition { Message = mealAddition.Message };
            await _context.StaticAdditions.AddAsync(mealAddition);
            await _context.SaveChangesAsync();
            return mealAddition;
        }

        public async Task<StaticMealAddition> DeleteMealAddition(StaticMealAddition mealAddition)
        {
            _imgService.DeleteImg(mealAddition);
            _context.Remove(mealAddition);
            await _context.SaveChangesAsync();
            return mealAddition;
        }

        public async Task<StaticMealAddition> GetMealAdditionByIdAsync(int id)
        {
            var mealaddition = await _context.StaticAdditions.SingleOrDefaultAsync(addition => addition.Id == id);
            if (mealaddition == null)
                return new StaticMealAddition { Message = $"There is no Addition with Id :{id}" };
            return mealaddition;
        }

        public  async Task<StaticMealAddition> UpdateMealAdditionAsync(StaticMealAddition mealAddition, UpdateStaticMealAdditionDto dto)
        {
            var errormessages = ValidateHelper<UpdateStaticMealAdditionDto>.Validate(dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new StaticMealAddition { Message = errormessages };
            }

            _imgService.UpdateImg(mealAddition, dto.AdditionImg);
            if (!string.IsNullOrEmpty(mealAddition.Message))
                return new StaticMealAddition { Message = mealAddition.Message };
            mealAddition.Name = dto.Name ?? mealAddition.Name;
            mealAddition.Price = dto.Price ?? mealAddition.Price;
            _context.Update(mealAddition);
            await _context.SaveChangesAsync();
            return mealAddition;
        }
    }
}
