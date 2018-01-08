using System;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Axerrio.BuildingBlocks;
using System.Threading;

namespace Axerrio.BuildingBlocks
{
    public class IdentifiedCommandHandler<TCommand> : CommandHandler<IdentifiedCommand<TCommand>>
        where TCommand : Command<TCommand>
    {
        private readonly IMediator _mediator;
        private readonly IClientRequestService _clientRequestService;

        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
            _clientRequestService = EnsureArg.IsNotNull(clientRequestService, nameof(clientRequestService));
        }

        public override async Task Handle(IdentifiedCommand<TCommand> message, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _clientRequestService.CreateClientRequestForCommandAsync<TCommand>(message.Id, cancellationToken);

            await _mediator.Send(message.Command);
        }
    }

    public class IdentifiedCommandHandler<TCommand, TResponse> : CommandHandler<IdentifiedCommand<TCommand, TResponse>, TResponse>
        where TCommand : Command<TCommand, TResponse>
    {
        private readonly IMediator _mediator;
        private readonly IClientRequestService _clientRequestService;

        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
            _clientRequestService = EnsureArg.IsNotNull(clientRequestService, nameof(clientRequestService));
        }

        protected virtual TResponse CreateResultForDuplicateRequest()
        {
            return default(TResponse);
        }

        public override async Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> message, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _clientRequestService.CreateClientRequestForCommandAsync<TCommand, TResponse>(message.Id, cancellationToken);

            return await _mediator.Send(message.Command);
        }
    }    
}