using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public class EFSettingService : ISettingService
    {
        private readonly SettingDbContext _context;
        private readonly ILogger<EFSettingService> _logger;

        public EFSettingService()
        {
        }

        public EFSettingService(SettingDbContext context, ILogger<EFSettingService> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task<Setting> FindByKeyAsync(string key)
        {
            return _context.Settings.SingleOrDefaultAsync(s => s.Key == key);
        }

        public async Task RemoveAsync(string key)
        {
            var setting = await FindByKeyAsync(key);

            if (setting != null)
            {
                _context.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }

         public async Task<Setting> UpdateOrAddAsync<T>(string key, T value)
        {
            var setting = await FindByKeyAsync(key);

            if (setting == null)
                setting = Setting.Create(key, value);
            else
                setting.SetValue(value);

            _context.Update(setting);

            await _context.SaveChangesAsync();

            return setting;
        }
    }
}
