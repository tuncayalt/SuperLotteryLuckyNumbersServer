using CloudantDotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface ICekilisCloudantService
    {
        Task<dynamic> CreateAsync(Cekilis item);
        Task<dynamic> DeleteAsync(Cekilis item);
        Task<dynamic> GetAsync();
        Task PopulateTestData();
        Task<string> UpdateAsync(Coupon item);
    }
}
