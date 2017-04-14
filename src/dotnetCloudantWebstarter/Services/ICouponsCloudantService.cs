using CloudantDotNet.Models;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface ICouponsCloudantService
    {
        Task<dynamic> CreateAsync(Coupon item);
        Task<dynamic> CreateListAsync(CouponList items);
        Task<dynamic> DeleteAsync(Coupon item);
        Task<dynamic> GetAllAsync();
        Task PopulateTestData();
        Task<string> UpdateAsync(Coupon item);
    }
}