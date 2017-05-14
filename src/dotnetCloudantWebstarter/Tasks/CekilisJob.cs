﻿using CloudantDotNet.Models;
using CloudantDotNet.Services;
using System;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public class CekilisJob : IJob
    {
        public DayOfWeek[] workDay { get; set; } = { DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday };
        public int startHour { get; set; } = 0;
        public int startMin { get; set; } = 0;
        public int endHour { get; set; } = 23;
        public int endMin { get; set; } = 59;
        public TimeSpan onceIn { get; set; } = TimeSpan.FromMinutes(10);
        public DateTime lastWorked { get; set; }

        ICekilisCloudantService _cloudantService;
        IMilliPiyangoService _mpService;

        public event EventHandler<CekilisEventArgs> onYeniCekilis;

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

                if (DateTime.UtcNow.GetTurkeyTime() - cekilisDb.GetDateTime() <= TimeSpan.FromDays(6))
                    return;

                DateTime dateInMP = await GetDateFromMP();

                if (dateInMP - cekilisDb.GetDateTime() <= TimeSpan.FromDays(2))
                    return;

                Cekilis cekilisMP = await GetCekilisFromMP(dateInMP);

                Cekilis cekilisInserted = await InsertCekilisToDB(cekilisMP);
                if (cekilisInserted != null && !string.IsNullOrWhiteSpace(cekilisInserted.numbers))
                {
                    InvokeYeniCekilisEvent(cekilisInserted);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("CekilisJob exception" + ex.Message);
            }
        }

        private void InvokeYeniCekilisEvent(Cekilis cekilisInserted)
        {
            CekilisEventArgs args = new CekilisEventArgs()
            {
                numbers = cekilisInserted.numbers
            };
            onYeniCekilis(this, args);
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

    public class CekilisEventArgs : EventArgs
    {
        internal string numbers;
    }
}
