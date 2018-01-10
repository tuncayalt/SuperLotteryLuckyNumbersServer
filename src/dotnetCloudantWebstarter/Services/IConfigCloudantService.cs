using dotnetCloudantWebstarter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCloudantWebstarter.Services
{
    public interface IConfigCloudantService
    {
        Task<Config> GetAsync();
    }
}
