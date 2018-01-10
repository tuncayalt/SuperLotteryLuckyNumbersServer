using dotnetCloudantWebstarter.Models;
using dotnetCloudantWebstarter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCloudantWebstarter.Cache
{
    public class ConfigCache
    {
        private static Config _config = new Config();

        public static Config config
        {
            get
            {
                return _config;
            }
        }

        private IConfigCloudantService _cloudantService;

        public ConfigCache(IConfigCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
            FillConfigAsync().Wait();
        }

        public async Task FillConfigAsync()
        {
            _config = await _cloudantService.GetAsync();
        }
    }
}
