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
        protected readonly TContext _context;
        protected readonly Action<DbContextOptionsBuilder<TContext>> _optionsAction;
        protected readonly ILoggerFactory _loggerFactory;

        protected EFSettingConfigurationSource(ILoggerFactory loggerFactory)
        {
            _loggerFactory = EnsureArg.IsNotNull(loggerFactory, nameof(LoggerFactory));
        }

        public EFSettingConfigurationSource(Action<DbContextOptionsBuilder<TContext>> optionsAction, ILoggerFactory loggerFactory): this(loggerFactory)
        {
            _optionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
        }

        public EFSettingConfigurationSource(TContext context, ILoggerFactory loggerFactory): this(loggerFactory)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (_optionsAction != null)
                return new EFSettingConfigurationProvider<TContext, TSettingService>(_optionsAction, _loggerFactory);

            return new EFSettingConfigurationProvider<TContext, TSettingService>(_context, _loggerFactory);
        }
    }
}