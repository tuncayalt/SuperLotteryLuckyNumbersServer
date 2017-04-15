﻿using CloudantDotNet.Models;
using CloudantDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public class CekilisJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Friday };
        public int startHour { get; set; } = 10;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(1);
        public DateTime lastWorked { get; set; }

        ICekilisCloudantService _cloudantService;
        IMilliPiyangoService _mpService;

        public CekilisJob(ICekilisCloudantService cloudantService, IMilliPiyangoService mpService)
        {
            _cloudantService = cloudantService;
            _mpService = mpService;
        }

        public void StartJob()
        {
            UpdateCekilis();

            Console.WriteLine("CekilisJob ran:" + DateTime.UtcNow.GetTurkeyTime());
        }

        private async void UpdateCekilis()
        {
            try
            {
                Cekilis cekilisDb = await GetCekilisFromDB();

                if (DateTime.UtcNow.GetTurkeyTime() - cekilisDb.GetDateTime() <= TimeSpan.FromDays(5))
                    return;

                DateTime dateInMP = await GetDateFromMP();

                if (dateInMP - cekilisDb.GetDateTime() <= TimeSpan.FromDays(2))
                    return;

                Cekilis cekilisMP = await GetCekilisFromMP(dateInMP);

                await InsertCekilisToDB(cekilisMP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CekilisJob UpdateCekilis exception" + ex.Message);
            }
        }

        private async Task<Cekilis> InsertCekilisToDB(Cekilis cekilisMP)
        {
            return await _cloudantService.CreateAsync(cekilisMP);
        }

        private async Task<Cekilis> GetCekilisFromMP(DateTime dateInMP)
        {
            return await _mpService.GetCekilisFromMP(dateInMP);
        }

        private async Task<DateTime> GetDateFromMP()
        {
            return await _mpService.GetDateFromMP();
        }

        private async Task<Cekilis> GetCekilisFromDB()
        {
            return await _cloudantService.GetAsync();
        }
    }
}
