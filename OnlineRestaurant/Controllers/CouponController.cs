using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("GetAllCoupons")]
        public async Task<IActionResult> GetAllCoupons() 
        { 
            return Ok(await _couponService.GetAllCouponsAsync());
        }
        [HttpGet("GetDiscountPercentage")]
        public IActionResult GetDiscountPercentage([FromQuery] CouponCodeDto couponCode)
        {
            if(ModelState.IsValid)
            {
                var discountPresentage = _couponService.GetDiscountPercentageAsync(couponCode.Code);
                if (discountPresentage == 0)
                    return NotFound("هذا الكوبون غير موجود او تم انتهاء صلاحيته");
                return Ok(discountPresentage);
            }
            return BadRequest(couponCode);
        }
        [HttpPost("AddCoupon")]
        public async Task<IActionResult> AddCoupon([FromBody]CouponDto coupon)
        {
            if(ModelState.IsValid)
            {
                var newCoupon = await _couponService.CreateCouponAsync(coupon);
                if (!string.IsNullOrEmpty(newCoupon.Message))
                    return BadRequest(newCoupon.Message);
                return Ok(newCoupon);
            }
            return BadRequest(coupon);
        }
        [HttpPut("UpdateCoupon/{id}")]
        public async Task<IActionResult> UpdateCoupon(int id,[FromBody]CouponDto coupon)
        {
            if (ModelState.IsValid)
            {
                var updatedCoupon = await _couponService.UpdateCouponAsync(id,coupon);
                if (!string.IsNullOrEmpty(updatedCoupon.Message))
                   return BadRequest(updatedCoupon.Message);
                return Ok(updatedCoupon);
            }
            return BadRequest(coupon);
        }
        [HttpDelete("DeleteCoupon/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon=await _couponService.DeleteCouponAsync(id);
            if (!string.IsNullOrEmpty(coupon.Message))
                return NotFound(coupon.Message);
            return Ok(coupon);
        }
    }
}
