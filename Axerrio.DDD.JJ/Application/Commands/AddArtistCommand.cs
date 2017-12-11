using Axerrio.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Commands
{
    public class AddArtistCommand : Command<AddArtistCommand>
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        public AddArtistCommand(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return FirstName;
            yield return LastName;
            yield return EmailAddress;
        }
    }
}
