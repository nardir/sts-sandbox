using MediatR;

namespace Axerrio.BuildingBlocks
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResponse>: ICommand, IRequest<TResponse>
    {
    }
}
