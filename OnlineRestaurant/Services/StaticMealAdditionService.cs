using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System;

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
            Dto.AdditionUrl=_imgService.Upload(Dto.AdditionImg);
            await _context.StaticAdditions.AddAsync(Dto);
            await _context.SaveChangesAsync();
            return _mapper.Map<StaticMealAdditionView>(Dto);
        }

        public async Task<StaticMealAdditionView> DeleteMealAddition(int id)
        {   var mealAddition= await _context.StaticAdditions.SingleOrDefaultAsync(addition => addition.Id == id); 
            if (mealAddition == null)
                return new StaticMealAdditionView { Message = $"There is no Addition with Id :{id}" };
            _imgService.Delete(mealAddition.AdditionUrl);
            _context.Remove(mealAddition);
            await _context.SaveChangesAsync();
            return _mapper.Map<StaticMealAdditionView>(mealAddition);
        }

        public async Task<StaticMealAdditionView> GetMealAdditionByIdAsync(int id)
        {
            var mealAddition=_mapper.Map<StaticMealAdditionView >( await _context.StaticAdditions.SingleOrDefaultAsync(addition => addition.Id == id));
            if (mealAddition == null)
                return new StaticMealAdditionView { Message = $"There is no Addition with Id :{id}" };
            return mealAddition;
        }
       

        public async Task<StaticMealAdditionView> UpdateMealAdditionAsync(int id, UpdateStaticMealAdditionDto dto)
        {
            var mealAddition = await _context.StaticAdditions.SingleOrDefaultAsync(addition => addition.Id == id);
            if (mealAddition == null)
                return new StaticMealAdditionView { Message = $"There is no Addition with Id :{id}" };
            _mapper.Map(dto,mealAddition);
            mealAddition.AdditionUrl=dto.AdditionImg == null ? mealAddition.AdditionUrl : _imgService.Update(mealAddition.AdditionUrl, dto.AdditionImg);
            await _context.SaveChangesAsync();
            return _mapper.Map<StaticMealAdditionView>(mealAddition);
        }

        public IEnumerable<StaticMealAdditionView> GetAllAdditions(PaginateDto dto)
        {
            return _mapper.Map<IEnumerable<StaticMealAdditionView>>(_context.StaticAdditions.Paginate(dto.Page, dto.Size));
        }
    }
}
