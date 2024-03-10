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
            var coupons=await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
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
                if (newCoupon.CouponCode == null)
                    return BadRequest("الكوبون لا يمكن ان يتكرر");
                return Ok(newCoupon);
            }
            return BadRequest(coupon);
        }
        [HttpPut("UpdateCoupon/{id}")]
        public async Task<IActionResult> UpdateCoupon(int id,[FromBody]UpdateCouponDto coupon)
        {
            if (ModelState.IsValid)
            {
                var updatedCoupon = await _couponService.UpdateCouponAsync(id,coupon);
                if (updatedCoupon.CouponCode == null)
                    return BadRequest("لم يتم العثور علي اي كوبون");
                if (updatedCoupon.CouponCode == "الكوبون لا يمكن ان يتكرر")
                    return BadRequest("الكوبون لا يمكن ان يتكرر");
                return Ok(updatedCoupon);
            }
            return BadRequest(coupon);
        }
        [HttpDelete("DeleteCoupon/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon=await _couponService.DeleteCouponAsync(id);
            if (coupon.CouponCode == null)
                return BadRequest("لم يتم العثور علي اي كوبون");
            return Ok(coupon);
        }
    }
}
