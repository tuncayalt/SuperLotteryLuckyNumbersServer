using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudantDotNet.Models;
using CloudantDotNet.Services;
using dotnetCloudantWebstarter.Cache;
using System.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudantDotNet.Controllers
{
    [Route("api/[controller]")]
    public class CekilisController : Controller
    {
        private readonly ICekilisCloudantService _cloudantService;
        //string oyunTuru = "superloto";
        //string urlTarihler = @"http://www.millipiyango.gov.tr/sonuclar/listCekilisleriTarihleri.php?tur=";
        //string urlNumaralar = @"http://www.millipiyango.gov.tr/sonuclar/cekilisler/";

        public CekilisController(ICekilisCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
        }

        // GET: api/cekilis
        [HttpGet]
        public dynamic GetLast()
        {
            return CekilisCache.cekilisList.Last();
        }

        // POST api/cekilis
        [HttpPost]
        public async Task<dynamic> Post([FromBody]Cekilis item)
        {
            return await _cloudantService.CreateAsync(item);
        }
    }
}
