using EnsureThat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    public class IdentifiedCommandHandler<TCommand, TResult> : IAsyncRequestHandler<IdentifiedCommand<TCommand, TResult>, TResult>
        where TCommand : Command<TCommand, TResult>
    {
        private readonly IMediator _mediator;
        private readonly IClientRequestService _clientRequestService;

        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
            _clientRequestService = EnsureArg.IsNotNull(clientRequestService, nameof(clientRequestService));
        }

        protected virtual TResult CreateResultForDuplicateRequest()
        {
            return default(TResult);
        }

        public async Task<TResult> Handle(IdentifiedCommand<TCommand, TResult> message)
        {
            await _clientRequestService.CreateClientRequestForCommandAsync<TCommand>(message.Id);

            // Send the embeded business command to mediator so it runs its related CommandHandler 
            var result = await _mediator.Send(message.Command);

            return result;
        }
    }
}