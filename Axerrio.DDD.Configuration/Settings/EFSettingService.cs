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
            {
                _logger.LogDebug($"Could not add setting. Setting with key {key} already exists");

                return null;
            }

            setting = Setting.Create(key, value);

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"Setting added with key {setting.Key} and value {setting.Value}");

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

                _logger.LogDebug($"Setting removed with key {key} and value {setting.Value}");
            }
            else
            {
                _logger.LogDebug($"Could not remove setting. Setting with key {key} not found");
            }
        }

         public async Task<Setting> UpdateOrAddAsync<T>(string key, T value)
        {
            bool updated = true;
            var setting = await FindByKeyAsync(key);

            if (setting == null)
            {
                setting = Setting.Create(key, value);
                updated = false;
            }
            else
            {
                setting.SetValue(value);
            }

            _context.Settings.Update(setting);

            await _context.SaveChangesAsync();

            string action = updated ? "updated" : "added";
            _logger.LogDebug($"Setting {action} with key {setting.Key} and value {setting.Value}");

            return setting;
        }
    }
}