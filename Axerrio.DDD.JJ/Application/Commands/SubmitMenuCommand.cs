using Axerrio.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Commands
{
    
    public class SubmitMenuCommand : Command<SubmitMenuCommand, bool>
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string RequesterName { get; set; }

        public SubmitMenuCommand(string description, string requesterName)
        {
            Description = description;
            RequesterName = requesterName;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Description;
            yield return RequesterName;
        }
    }
}
