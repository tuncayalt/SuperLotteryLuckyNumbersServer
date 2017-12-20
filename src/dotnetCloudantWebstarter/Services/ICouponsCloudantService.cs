using CloudantDotNet.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CloudantDotNet.Services
{
    public interface ICouponsCloudantService
    {
        Task<dynamic> CreateAsync(Coupon item);
        Task<dynamic> CreateListAsync(CouponList items);
        Task<dynamic> DeleteAsync(string couponId);
        Task<dynamic> GetAllAsync();
        Task<dynamic> GetAllByUserName(string userName);
        Task<dynamic> GetAllByTarih(string tarih);
        Task<dynamic> GetAllByUserNameAndTarih(string userName, string tarih);
        Task PopulateTestData();
        Task<string> UpdateAsync(Coupon item);
        Task<dynamic> UpdateBulkAsync(CouponListDto items);
        Task<List<CouponDto>> GetWithLimitByTarih(string tarih, int updateCouponCount);
        Task<dynamic> DeleteBulkAsync(CouponListToDeleteDto items);
        Task<dynamic> GetListByCouponIds(List<string> couponIds);
    }
}