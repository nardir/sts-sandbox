using Axerrio.DDD.BuildingBlocks;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    public interface ICommand { }

    public abstract class Command<TCommand, TResult>: ValueObject<TCommand>, ICommand, IRequest<TResult>
        where TCommand : Command<TCommand, TResult>
    {
        [JsonIgnore]
        public bool Initiating { get; private set; } = true;

        protected Command() {}

        public Command(bool initiating)
        {
            Initiating = initiating;
        }
    }
}
