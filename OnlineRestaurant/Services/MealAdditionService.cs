using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class MealAdditionService : IMealAdditionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MealAdditionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MealAddition> CreateMealAddition(MealAddition Dto)
        {
            if (!await _context.Meals.AnyAsync(meal => meal.Id == Dto.MealId))
                return new MealAddition { Message = $"There is no Meal with Id : {Dto.MealId}!" };

            await _context.AddAsync(Dto);
            await _context.SaveChangesAsync();
            return Dto;
        }

        public async Task<MealAddition> DeleteMealAddition(int id)
        {
            var mealAddition = await GetMealAdditionByIdAsync(id);
            if (mealAddition == null)
                return new MealAddition { Message = $"There is no MealAddition with Id:{id} " };
            _context.Remove(mealAddition);
            await _context.SaveChangesAsync();
            return mealAddition;
        }

        private async Task<MealAddition> GetMealAdditionByIdAsync(int id)
        {
            return await _context.MealAdditions.Include(c=>c.Choices).SingleOrDefaultAsync(m => m.Id == id);
        }

        public IEnumerable<MealAddition> GetMealAdditions(int id, PaginateDto dto)
        {
            if (!_context.Meals.Any(m => m.Id == id))
                return new List<MealAddition> { new MealAddition { Message = $"No meal is found with Id:{id}" } };
            return _context.MealAdditions.Include(c=>c.Choices).Where(c=>c.MealId==id).Paginate(dto.Page, dto.Size).ToList();
        }

        public async Task<MealAddition> UpdateMealAdditionAsync(int id, UpdateMealAdditionDto dto)
        {
            var mealAddition=await GetMealAdditionByIdAsync(id);
            if (mealAddition == null)
                return new MealAddition { Message = $"There is no MealAddition with Id:{id} " };
            if (!await _context.Meals.AnyAsync(meal => meal.Id == dto.MealId))
                return new MealAddition { Message = $"There is no Meal with Id : {dto.MealId}!" };

            _mapper.Map(dto, mealAddition);
            await _context.SaveChangesAsync();
            return mealAddition;
        }
        public async Task<string> DeleteChoiceAsync(int ChoiceId)
        {
            var choice =await _context.Choices.FirstOrDefaultAsync(c=>c.Id==ChoiceId);
            if (choice == null)
                return "لم يتم العثور علي اي اختيارات";

            _context.Remove(choice);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        public async Task<string> AddChoiceAsync(Choice choice)
        {
            if (!await _context.MealAdditions.AnyAsync(ma => ma.Id == choice.MealAdditionId))
                return $"There is no MealAddition with Id : {choice.MealAdditionId}!";
            await _context.Choices.AddAsync(choice);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
       
    }

}            


