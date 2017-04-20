﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Tasks
{
    public interface IJobManager
    {
        void AddJob(IJob job);
        void RemoveJob(IJob job);
    }
}