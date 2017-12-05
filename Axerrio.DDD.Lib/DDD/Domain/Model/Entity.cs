using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Axerrio.BuildingBlocks
{
    public abstract class Entity<TIdentity>: IEntity, IDomainEventsEntity
    {
        public Entity()
        {
            _domainEvents = new List<DomainEvent>();
        }

        [JsonProperty]
        public virtual TIdentity Identity { get; protected set; }

        public override int GetHashCode()
        {
            return Identity.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TIdentity>))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity<TIdentity> item = (Entity<TIdentity>)obj;

            return Identity.Equals(item.Identity);
        }

        public static bool operator ==(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            if (ReferenceEquals(left, null))
                return (ReferenceEquals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            return !(left == right);
        }

        #region DomainEvents

        private readonly List<DomainEvent> _domainEvents;

        [JsonIgnore]
        public ReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent @event)
        {
            _domainEvents.Add(@event);
        }
        public void RemoveDomainEvent(DomainEvent @event)
        {
            _domainEvents.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        #endregion 
    }
}