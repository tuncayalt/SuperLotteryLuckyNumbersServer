using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudantDotNet.Services;
using CloudantDotNet.Models;

namespace CloudantDotNet.Controllers
{
    [Route("api/[controller]")]
    public class CouponController : Controller
    {
        private readonly ICouponsCloudantService _cloudantService;

        public CouponController(ICouponsCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
        }
        // GET: api/Coupon
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/Coupon/5
        [HttpGet("{user}")]
        public async Task<dynamic> Get(string user)
        {
            return await _cloudantService.GetAllByUserName(user);
        }

        // POST api/Coupon
        [HttpPost]
        public async Task<dynamic> Post([FromBody]Coupon[] items)
        {
            CouponList list = new CouponList();
            list.docs = items.ToList();
            return await _cloudantService.CreateListAsync(list);
        }

        // PUT api/Coupon/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/Coupon/5
        [HttpDelete("{couponId}")]
        public async Task<List<string>> Delete(string couponId)
        {
            if (couponId.Contains("["))
            {
                string couponIdTemp = couponId.Replace("[", "").Replace("]", "");
                if (couponIdTemp == null || string.IsNullOrWhiteSpace(couponIdTemp))
                {
                    return new List<string>();
                }

                string[] couponIdArray = couponIdTemp.Split(',');
                if (couponIdArray == null || !couponIdArray.Any())
                {
                    return new List<string>();
                }

                List<string> couponIdList = couponIdArray.Select(c => c.Trim()).ToList();

                List<CouponToDeleteDto> couponList = await _cloudantService.GetListByCouponIds(couponIdList);
                couponList.ForEach(c => c._deleted = true);

                CouponListToDeleteDto couponListDto = new CouponListToDeleteDto();
                couponListDto.docs = couponList;
                List<CouponAfterDeleteDto> couponsDeleteResponse = await _cloudantService.DeleteBulkAsync(couponListDto);
                List<string> couponIdsDeleted = couponList.Where(c => couponsDeleteResponse.Where(a => a.ok).Select(c1 => c1.id).ToList().Contains(c._id)).Select(o => o.CouponId).ToList();
                return couponIdsDeleted;
            }
            else
            {
                return await _cloudantService.DeleteAsync(couponId);
            }
            
        }
    }
}
