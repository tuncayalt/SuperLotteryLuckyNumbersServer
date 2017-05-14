using CloudantDotNet.Models;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface IPushService
    {
        Task<bool> SendPush(PushNotification push);
    }
}
