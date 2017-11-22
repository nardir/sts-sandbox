using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
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
