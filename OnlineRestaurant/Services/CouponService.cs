using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CouponService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Coupon> CreateCouponAsync(CouponDto coupon)
        {
            if( _context.Coupons.AsEnumerable().Any(c=> c.CouponCode.Equals(coupon.CouponCode.Trim(), StringComparison.Ordinal)))
                return new Coupon { Message = "الكوبون لا يمكن ان يتكرر" };
            var newCoupon=_mapper.Map<Coupon>(coupon);
            await _context.AddAsync(newCoupon);
            await _context.SaveChangesAsync();
            return newCoupon;
        }

        public async Task<Coupon> DeleteCouponAsync(int couponId)
        {
            var coupon=await _context.Coupons.SingleOrDefaultAsync(c=>c.Id==couponId);
            if(coupon==null)
                return new Coupon { Message = "لم يتم العثور علي اي كوبون" };
            _context.Remove(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            return await _context.Coupons.ToListAsync();
        }

        public  decimal GetDiscountPercentageAsync(string code)
        {
            var coupon =  _context.Coupons.AsEnumerable().SingleOrDefault(c => c.CouponCode.Equals(code.Trim(), StringComparison.Ordinal));
            if(coupon== null)
                return 0;
            return coupon.DiscountPercentage;
        }

        public async Task<Coupon> UpdateCouponAsync(int id,CouponDto dto)
        {
            var coupon = await _context.Coupons.SingleOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
                return new Coupon { Message="لم يتم العثور علي اي كوبون"};
            if(dto.CouponCode.Trim() != coupon.CouponCode)
            {
                if (_context.Coupons.AsEnumerable().Any(c => c.CouponCode.Equals(dto.CouponCode.Trim(), StringComparison.Ordinal)))
                    return new Coupon { Message = "الكوبون لا يمكن ان يتكرر" };
            }
            _mapper.Map(dto, coupon);    
            await _context.SaveChangesAsync();
            return coupon;
        }
    }
}
