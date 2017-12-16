using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public interface ISettingService
    {
        Task<Setting> UpdateOrAddAsync<T>(string key, T value);
        Task RemoveAsync(string key);
        Task<Setting> FindByKeyAsync(string key);
    }
}
