using Axerrio.DDD.Ordering.BuildingBlocks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Axerrio.DDD.Lib
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
