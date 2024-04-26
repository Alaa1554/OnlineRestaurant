using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon> CreateCouponAsync(CouponDto coupon);
        Task<Coupon> UpdateCouponAsync(int id,CouponDto dto);
        Task<Coupon> DeleteCouponAsync(int couponId);
        decimal GetDiscountPercentageAsync(string code);
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();
    }
}
