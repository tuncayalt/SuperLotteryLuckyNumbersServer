using CloudantDotNet.Models;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface ICekilisCloudantService
    {
        Task<Cekilis> CreateAsync(Cekilis item);
        Task<dynamic> DeleteAsync(Cekilis item);
        Task<Cekilis> GetAsync();
        Task PopulateTestData();
        Task<string> UpdateAsync(Coupon item);
    }
}
