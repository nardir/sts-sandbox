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

        protected EFSettingService()
        {
        }

        public EFSettingService(SettingDbContext context, ILogger<EFSettingService> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public async Task<Setting> AddAsync<T>(string key, T value)
        {
            var setting = await FindByKeyAsync(key);

            if (setting != null)
                return null;

            setting = Setting.Create(key, value);

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return setting;
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
                _context.Settings.Remove(setting);
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

            _context.Settings.Update(setting);

            await _context.SaveChangesAsync();

            return setting;
        }
    }
}
