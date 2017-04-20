using CloudantDotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface IUserCloudantService
    {
        Task<dynamic> CreateAsync(User item);
        Task<dynamic> DeleteAsync(User item);
        Task<dynamic> GetTokenAsync(string token);
        Task<dynamic> GetUserAsync(string userMail);
        Task<List<User>> GetPushCekilis();
        Task<string> UpdateAsync(User item);
    }
}
