using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IDomainEventsEntity<TDomainEvent>
    {
        ReadOnlyCollection<TDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }

    public abstract class Entity<TIdentity, TDomainEvent>: IDomainEventsEntity<TDomainEvent>
    {
        public Entity()
        {
            _domainEvents = new List<TDomainEvent>();
        }

        [JsonProperty]
        public virtual TIdentity Identity { get; protected set; }

        public override int GetHashCode()
        {
            return Identity.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TIdentity, TDomainEvent>))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity<TIdentity, TDomainEvent> item = (Entity<TIdentity, TDomainEvent>)obj;

            return Identity.Equals(item.Identity);
        }

        public static bool operator ==(Entity<TIdentity, TDomainEvent> left, Entity<TIdentity, TDomainEvent> right)
        {
            if (ReferenceEquals(left, null))
                return (ReferenceEquals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TIdentity, TDomainEvent> left, Entity<TIdentity, TDomainEvent> right)
        {
            return !(left == right);
        }

        #region DomainEvents

        private readonly List<TDomainEvent> _domainEvents;
        [JsonIgnore]
        public ReadOnlyCollection<TDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(TDomainEvent @event)
        {
            _domainEvents.Add(@event);
        }
        public void RemoveDomainEvent(TDomainEvent @event)
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