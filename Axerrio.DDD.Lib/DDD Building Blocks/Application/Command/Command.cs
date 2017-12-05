namespace Axerrio.BuildingBlocks.Command
{
    public abstract class Command<TCommand>: ValueObject<TCommand>, ICommand
        where TCommand: Command<TCommand>
    {
        public bool Initiating { get; private set; } = true;

        protected Command()
        {

        } 

        public Command(bool initiating)
        {
            Initiating = initiating;
        }
    }

    public abstract class Command<TCommand, TResponse>: Command<TCommand>, ICommand<TResponse>
        where TCommand : Command<TCommand, TResponse>
    {
        protected Command() { }

        public Command(bool initiating): base(initiating)
        {
        }
    }

    public abstract class Command : Command<Command, bool>
    {
        protected Command() { }

        public Command(bool initiating): base(initiating)
        {
        }
    }
}
