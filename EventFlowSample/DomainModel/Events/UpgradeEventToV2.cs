using EventFlow.Aggregates;
using EventFlow.EventStores;
using System.Collections.Generic;

namespace DomainModel.Events
{
    public class UpgradeEventToV2 : IEventUpgrader<Aggregate, AggregateId>
    {
        private readonly IDomainEventFactory domainEventFactory;

        public UpgradeEventToV2(IDomainEventFactory domainEventFactory)
        {
            this.domainEventFactory = domainEventFactory;
        }

        public IEnumerable<IDomainEvent<Aggregate, AggregateId>> Upgrade(IDomainEvent<Aggregate, AggregateId> domainEvent)
        {
            var eventV1 = domainEvent as IDomainEvent<Aggregate, AggregateId, Event>;

            yield return eventV1 == null
                ? domainEvent
                : domainEventFactory.Upgrade<Aggregate, AggregateId>(domainEvent, new EventV2(eventV1.AggregateEvent.MagicNumber));
        }
    }
}