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
                return new Coupon();
            var newCoupon=_mapper.Map<Coupon>(coupon);
            await _context.AddAsync(newCoupon);
            await _context.SaveChangesAsync();
            return newCoupon;
        }

        public async Task<Coupon> DeleteCouponAsync(int couponId)
        {
            var coupon=await _context.Coupons.SingleOrDefaultAsync(c=>c.Id==couponId);
            if(coupon==null)
                return new Coupon();
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

        public async Task<Coupon> UpdateCouponAsync(int id,UpdateCouponDto updateCoupon)
        {
            var coupon = await _context.Coupons.SingleOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
                return new Coupon();
            if (updateCoupon.CouponCode != null)
            {
                if ( _context.Coupons.AsEnumerable().Any(c => c.CouponCode.Equals(updateCoupon.CouponCode.Trim(), StringComparison.Ordinal))&&updateCoupon.CouponCode.Trim()!=coupon.CouponCode)
                    return new Coupon { CouponCode = "الكوبون لا يمكن ان يتكرر" };
                coupon.CouponCode = updateCoupon.CouponCode.Trim();
            }
            coupon.DiscountPercentage=updateCoupon.DiscountPercentage??coupon.DiscountPercentage;
            await _context.SaveChangesAsync();
            return coupon;
        }
    }
}
