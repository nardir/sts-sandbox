using Axerrio.DDD.BuildingBlocks;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Model
{
    public class ConfigurationSection: ValueObject<ConfigurationSection>
    {
        public string Name { get; private set; }

        protected ConfigurationSection()
        {

        }

        public ConfigurationSection(string name)
        {
            Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Name;
        }
    }
}
