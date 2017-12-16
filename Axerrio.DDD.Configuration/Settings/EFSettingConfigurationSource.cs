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
    public class EFSettingConfigurationSource<TContext> : IConfigurationSource
        where TContext : DbContext, ISettingDbContext, new()
    {
        private TContext _context;

        protected Action<DbContextOptionsBuilder<TContext>> OptionsAction { get; set; }

        public EFSettingConfigurationSource(Action<DbContextOptionsBuilder<TContext>> optionsAction)
        {
            OptionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
        }

        public EFSettingConfigurationSource(TContext context)
        {
            _context = context;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EFSettingConfigurationProvider<TContext> provider;

            if (_context == null)
                if (OptionsAction == null)
                    provider = new EFSettingConfigurationProvider<TContext>(context: null);
                else
                    provider = new EFSettingConfigurationProvider<TContext>(OptionsAction);
            else
                provider = new EFSettingConfigurationProvider<TContext>(_context);

            return provider;
        }
    }
}