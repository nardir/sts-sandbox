using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public interface IJsonEFConfigurationService
    {
        //Task AddAsync<T>(string key, T value);
        //Task UpdateAsync<T>(string key, T value);
        Task RemoveAsync(string key);
        Task SaveAsync<T>(string key, T value);
    }
}