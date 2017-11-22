using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    public class IdentifiedCommand<TCommand, TResult> : Command<IdentifiedCommand<TCommand, TResult>, TResult>
        where TCommand : Command<TCommand, TResult>
    {
        public TCommand Command { get; }
        public Guid Id { get; }

        protected IdentifiedCommand() { }

        public IdentifiedCommand(TCommand command, Guid id, bool initiating = true): base(initiating)
        {
            EnsureArg.IsNotNull(command, nameof(command));
            EnsureArg.IsNotEmpty(id, nameof(id));

            Id = id;
            Command = command;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Id;
        }
    }
}
