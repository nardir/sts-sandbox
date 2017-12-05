using System;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Axerrio.BuildingBlocks.Command;

namespace Axerrio.BuildingBlocks
{
    public class IdentifiedCommandHandler<TCommand> : IdentifiedCommandHandler, ICommandHandler<IdentifiedCommand<TCommand>>
        where TCommand : Command<TCommand>
    {
        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService): base(mediator, clientRequestService)
        {
        }

        public Task Handle(IdentifiedCommand<TCommand> message)
        {
            return Handle<TCommand>(message.Id, message.Command);
        }
    }

    public class IdentifiedCommandHandler<TCommand, TResponse> : IdentifiedCommandHandler, ICommandHandler<IdentifiedCommand<TCommand, TResponse>, TResponse>
        where TCommand : Command<TCommand, TResponse>
    {
        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService): base(mediator, clientRequestService)
        {
        }

        protected virtual TResponse CreateResultForDuplicateRequest()
        {
            return default(TResponse);
        }

        public Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> message)
        {
            return Handle<TCommand, TResponse>(message.Id, message.Command);            
        }
    }

    public abstract class IdentifiedCommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IClientRequestService _clientRequestService;

        public IdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
            _clientRequestService = EnsureArg.IsNotNull(clientRequestService, nameof(clientRequestService));
        }

        protected async Task Handle<TCommand>(Guid id, IRequest message) where TCommand: ICommand
        {
            await _clientRequestService.CreateClientRequestForCommandAsync<TCommand>(id);

            await _mediator.Send(message);
        }

        protected async Task<TResponse> Handle<TCommand, TResponse>(Guid id, IRequest<TResponse> message) where TCommand : ICommand
        {
            await _clientRequestService.CreateClientRequestForCommandAsync<TCommand>(id);

            return await _mediator.Send(message);
        }
    }
}