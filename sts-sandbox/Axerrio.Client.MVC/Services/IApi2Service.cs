﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.Client.MVC.Services
{
    public interface IApi2Service
    {
        Task<string> GetValuesAsync();
    }
}
