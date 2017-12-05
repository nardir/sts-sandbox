using System;
using System.Collections.Generic;
using EnsureThat;
using Axerrio.BuildingBlocks.Command;

namespace Axerrio.BuildingBlocks
{
    public class IdentifiedCommand<TCommand, TResponse> : IdentifiedCommand<TCommand>, ICommand<TResponse> 
        where TCommand : Command<TCommand, TResponse>
    {

        protected IdentifiedCommand() { }

        public IdentifiedCommand(TCommand command, Guid id, bool initiating = true): base(command, id, initiating)
        {
        }
    }

    public class IdentifiedCommand<TCommand>: Command<IdentifiedCommand<TCommand>>, ICommand
        where TCommand : Command<TCommand>
    {
        public TCommand Command { get; private set; }
        public Guid Id { get; private set; }

        protected IdentifiedCommand() { }

        public IdentifiedCommand(TCommand command, Guid id, bool initiating = true) : base(initiating)
        {
            Command = EnsureArg.IsNotNull(command, nameof(command));
            Id = EnsureArg.IsNotEmpty(id, nameof(id));
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Id;
        }
    }
}
