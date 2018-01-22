using System;
using System.Collections.Generic;
using EnsureThat;

namespace Axerrio.BuildingBlocks
{
    public class IdentifiedCommand<TCommand, TResponse> : Command<IdentifiedCommand<TCommand, TResponse>, TResponse>, ICommand<TResponse>, IIdentifiedCommand<TCommand>
        where TCommand : Command<TCommand, TResponse>
    {
        public TCommand Command { get; private set; }

        public Guid Id { get; private set; }
        

        protected IdentifiedCommand() { }

        public IdentifiedCommand(TCommand command, Guid id, bool initiating = true) : base(initiating)
        {
            Command = command;
            Id = id;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Id;
        }
    }

    public class IdentifiedCommand<TCommand> : Command<IdentifiedCommand<TCommand>>, ICommand, IIdentifiedCommand<TCommand>
        where TCommand : Command<TCommand>
    {
        public TCommand Command { get; private set; }
        public Guid Id { get; private set; }

        protected IdentifiedCommand() { }

        public IdentifiedCommand(TCommand command, Guid id, bool initiating = true) : base(initiating)
        {
            Command = command;
            Id = id;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Id;
        }
    }
}
