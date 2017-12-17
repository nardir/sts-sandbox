using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public class EFSettingConfigurationSource<TContext, TSettingService> : IConfigurationSource
        where TContext : DbContext, ISettingDbContext, new()
        where TSettingService : ISettingService
    {
        private TContext _context;

        protected readonly Action<DbContextOptionsBuilder<TContext>> _optionsAction;
        protected readonly Func<ISettingService, Task> _seeder;

        public EFSettingConfigurationSource(Action<DbContextOptionsBuilder<TContext>> optionsAction, Func<ISettingService, Task> seeder = null)
        {
            _optionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
            _seeder = seeder;
        }

        public EFSettingConfigurationSource(TContext context, Func<ISettingService, Task> seeder = null)
        {
            _context = context;
            _seeder = seeder;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EFSettingConfigurationProvider<TContext, TSettingService> provider;

            if (_context == null)
                if (_optionsAction == null)
                    provider = new EFSettingConfigurationProvider<TContext, TSettingService>(context: null, seeder: _seeder);
                else
                    provider = new EFSettingConfigurationProvider<TContext, TSettingService>(_optionsAction, _seeder);
            else
                provider = new EFSettingConfigurationProvider<TContext, TSettingService>(_context, _seeder);

            return provider;
        }
    }
}