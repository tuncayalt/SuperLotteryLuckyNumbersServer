using CloudantDotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Services
{
    public interface IMilliPiyangoService
    {
        Task<dynamic> GetDateFromMP();
        Task<dynamic> GetCekilisFromMP(DateTime dateInMP);
    }
}
