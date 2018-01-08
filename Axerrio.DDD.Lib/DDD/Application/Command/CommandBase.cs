using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BuildingBlocks
{
    public abstract class CommandBase<TCommand> : ValueObject<TCommand>
        where TCommand : CommandBase<TCommand>
    {
        [JsonIgnore]
        public bool Initiating { get; private set; } = true;
        protected CommandBase() { }

        public CommandBase(bool initiating)
        {
            Initiating = initiating;
        }
    }
}
