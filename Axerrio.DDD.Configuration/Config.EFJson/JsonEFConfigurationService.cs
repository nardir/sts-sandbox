using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public class JsonEFConfigurationService : IJsonEFConfigurationService
    {
        JsonEFConfigurationContext _context;

        public JsonEFConfigurationService(JsonEFConfigurationContext context)
        {
            _context = context;
        }

        public async Task RemoveAsync(string key)
        {
            var configValue = await FindByKeyAsync(key);

            if (configValue != null)
            {
                _context.Remove(configValue);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync<T>(string key, T value)
        {
            var configValue = await FindByKeyAsync(key);

            var json = JsonConvert.SerializeObject(value);

            if (configValue == null)
                configValue = new JsonEFConfigurationValue(key);

            configValue.Value = json;

            _context.Update(configValue);

            await _context.SaveChangesAsync();
        }

        private Task<JsonEFConfigurationValue> FindByKeyAsync(string key)
        {
            return _context.Values.SingleOrDefaultAsync(v => v.Key == key);
        }
    }
}