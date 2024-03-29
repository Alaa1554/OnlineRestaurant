﻿
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

        public MealAdditionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MealAddition> CreateMealAddition(MealAddition Dto)
        {
            var errormessages = ValidateHelper<MealAddition>.Validate(Dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealAddition { Message = errormessages };
            }
            if (!await _context.Meals.AnyAsync(meal => meal.Id == Dto.MealId))
                return new MealAddition { Message = $"There is no Meal with Id : {Dto.MealId}!" };

            var mealAddition = new MealAddition
            {
                Name = Dto.Name,
                MealId = Dto.MealId,
                Choices = Dto.Choices
            };
           
            await _context.AddAsync(mealAddition);
           await _context.SaveChangesAsync();
            return mealAddition;
        }

        public async Task<MealAddition> DeleteMealAddition(MealAddition mealAddition)
        {
            _context.Remove(mealAddition);
            
            await _context.SaveChangesAsync();
            return mealAddition;
        }

        public async Task<MealAddition> GetMealAdditionByIdAsync(int id)
        {
            var mealAddition = await _context.MealAdditions.Include(c=>c.Choices).SingleOrDefaultAsync(m => m.Id == id);
            if (mealAddition == null)
                return new MealAddition { Message = $"There is no MealAddition with Id:{id} " };
            return mealAddition;

        }

        public IEnumerable<MealAddition> GetMealAdditions(int id, PaginateDto dto)
        {
            var mealAdditions =_context.MealAdditions.Include(c=>c.Choices).Where(c=>c.MealId==id).Paginate(dto.Page, dto.Size).ToList();
            return mealAdditions;

        }

        public async Task<MealAddition> UpdateMealAdditionAsync(MealAddition mealAddition, UpdateMealAdditionDto dto,int? id)
        {
            var errormessages = ValidateHelper<UpdateMealAdditionDto>.Validate(dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealAddition { Message = errormessages };
            }
             
            if (dto.MealId != null)
            {
                if (!await _context.Meals.AnyAsync(meal => meal.Id == dto.MealId))
                    return new MealAddition { Message = $"There is no Meal with Id : {dto.MealId}!" };

                mealAddition.MealId = dto.MealId??mealAddition.MealId;
            }
            
            mealAddition.Name = dto.Name ?? mealAddition.Name;
            if (id != null)
            {
                var choice= mealAddition.Choices.FirstOrDefault(c=>c.Id==id);
                if (choice == null)
                {
                    return new MealAddition { Message = $"There is no Choice with Id : {id}!" };
                }
                choice.Name= dto.Choice.Name ?? choice.Name;
                choice.Price=dto.Choice.Price??choice.Price;
            }
            _context.Update(mealAddition);

            await _context.SaveChangesAsync();

            return mealAddition;
        }
        public async Task<string> DeleteChoiceAsync(int AdditionId,int ChoiceId)
        {
            var Addition =await _context.MealAdditions.Include(c=>c.Choices).SingleOrDefaultAsync(m=>m.Id==AdditionId);
            if(Addition == null)
                return "لم يتم العثور علي اضافات";
            var Choice = Addition.Choices.FirstOrDefault(c=>c.Id==ChoiceId);
            if (Choice == null)
                return "لم يتم العثور علي اختيارات";

            Addition.Choices.Remove(Choice);

            _context.Update(Addition);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        public async Task<string> AddChoiceAsync(int AdditionId,Choice choice)
        {
            var Addition = await _context.MealAdditions.Include(c => c.Choices).SingleOrDefaultAsync(m => m.Id == AdditionId);
            if (Addition == null)
                return "لم يتم العثور علي اضافات";
            Addition.Choices.Add(choice);
            _context.Update(Addition);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
       
    }

}            


