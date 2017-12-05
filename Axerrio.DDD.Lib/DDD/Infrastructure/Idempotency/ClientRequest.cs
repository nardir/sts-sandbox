using System;
using EnsureThat;

namespace Axerrio.BuildingBlocks
{
    class ClientRequest
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime Time { get; private set; }

        protected ClientRequest() { }

        public ClientRequest(Guid id, string name, DateTime time)
        {
            Id = EnsureArg.IsNotEmpty(id, nameof(id));
            Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
            
            Time = time;
        }
    }
}
