using System;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface IMilliPiyangoService
    {
        Task<dynamic> GetDateFromMP();
        Task<dynamic> GetCekilisFromMP(DateTime dateInMP);
    }
}
