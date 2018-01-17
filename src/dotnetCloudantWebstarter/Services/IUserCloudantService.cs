using CloudantDotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface IUserCloudantService
    {
        Task<bool> CreateAsync(User item);
        Task<dynamic> DeleteAsync(User item);
        Task<dynamic> GetTokenAsync(string token);
        Task<dynamic> GetUserAsync(string userMail);
        Task<List<User>> GetPushCekilis();
        Task<bool> UpdateAsync(User item);
        Task<List<User>> GetAllByUserIds(List<string> list);
    }
}
