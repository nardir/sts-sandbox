using Axerrio.BB.DDD.Infrastructure.Hosting;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class TestHostedService : BackgroundService
    {
        ILogger<TestHostedService> _logger;

        public TestHostedService(ILogger<TestHostedService> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"Test background task is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 Test background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Test background task is doing background work.");

                _logger.LogDebug("Forwarding...");

                await Task.Delay(2000, stoppingToken);

                continue;
            }

            _logger.LogDebug($"Test background task is stopping.");

        }
    }
}