using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Hosting.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsForwarderJob : TimedJob
    {
        private readonly ILogger<IntegrationEventsForwarderJob> _logger;
        private readonly IIntegrationEventsForwarderService _integrationEventsForwarderService;

        public IntegrationEventsForwarderJob(ILogger<IntegrationEventsForwarderJob> logger, IIntegrationEventsForwarderService integrationEventsForwarderService)
            : base(logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _integrationEventsForwarderService = EnsureArg.IsNotNull(integrationEventsForwarderService, nameof(integrationEventsForwarderService));
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _integrationEventsForwarderService.ForwardAsync(cancellationToken);
        }
    }
}
