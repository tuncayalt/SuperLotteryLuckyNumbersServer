using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudantDotNet.Models;
using Newtonsoft.Json;
using System.Net;
using CloudantDotNet.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace dotnetCloudantWebstarter.Controllers
{
    [Route("api/[controller]")]
    public class CekilisController : Controller
    {
        private readonly ICekilisCloudantService _cloudantService;
        string oyunTuru = "superloto";
        string urlTarihler = @"http://www.millipiyango.gov.tr/sonuclar/listCekilisleriTarihleri.php?tur=";
        string urlNumaralar = @"http://www.millipiyango.gov.tr/sonuclar/cekilisler/";

        public CekilisController(ICekilisCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<dynamic> GetLast()
        {
            return await _cloudantService.GetAsync();
        }

        // POST api/values
        [HttpPost]
        public async Task<dynamic> Post([FromBody]Cekilis item)
        {
            return await _cloudantService.CreateAsync(item);
        }
    }
}
