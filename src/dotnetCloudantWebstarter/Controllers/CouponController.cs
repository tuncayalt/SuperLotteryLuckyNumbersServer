using System;
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
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
