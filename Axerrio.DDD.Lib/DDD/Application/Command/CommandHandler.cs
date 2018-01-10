using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public abstract Task Handle(TCommand message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
