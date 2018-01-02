using CloudantDotNet.Models;
using CloudantDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCloudantWebstarter.Cache
{
    public class CekilisCache
    {
        private static List<Cekilis> _cekilisList = new List<Cekilis>();

        public static List<Cekilis> cekilisList {
            get
            {
                return _cekilisList;
            }
        }

        private ICekilisCloudantService _cloudantService;

        public CekilisCache(ICekilisCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
            FillCekilisListAsync().Wait();
        }

        public async Task FillCekilisListAsync()
        {
            _cekilisList = await _cloudantService.GetAllAsync();
            _cekilisList.Sort((x, y) => x.GetDateTime().CompareTo(y.GetDateTime()));
        }

        public static void AddCekilis(Cekilis cekilis)
        {
            _cekilisList.Add(cekilis);
            _cekilisList.Sort((x, y) => x.GetDateTime().CompareTo(y.GetDateTime()));
        }
    }
}
