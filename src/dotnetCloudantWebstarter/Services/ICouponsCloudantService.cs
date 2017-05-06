using CloudantDotNet.Models;
using System.Threading.Tasks;

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
        Task PopulateTestData();
        Task<string> UpdateAsync(Coupon item);
    }
}