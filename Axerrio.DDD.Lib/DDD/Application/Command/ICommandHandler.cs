using MediatR;

namespace Axerrio.BuildingBlocks
{
    public interface ICommandHandler<TCommand> : IAsyncRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IAsyncRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
}
