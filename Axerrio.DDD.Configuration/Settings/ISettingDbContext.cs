using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public interface ISettingDbContext
    {
        DbSet<Setting> Settings { get; set; }
    }
}
