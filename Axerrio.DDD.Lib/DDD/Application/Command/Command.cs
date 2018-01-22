using Newtonsoft.Json;

namespace Axerrio.BuildingBlocks
{  

    public abstract class Command<TCommand> : CommandBase<TCommand>, ICommand
        where TCommand : Command<TCommand>
    {
        protected Command() { }

        public Command(bool initiating) : base(initiating)
        {
        }
    }

    public abstract class Command<TCommand, TResponse> : CommandBase<TCommand>, ICommand<TResponse>
        where TCommand : Command<TCommand, TResponse>
    {
        protected Command() { }

        public Command(bool initiating) : base(initiating)
        {
        }
    }
}
