using System.Collections.Generic;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace DomainModel.Events;

public class UpgradeEventToV2 : IEventUpgrader<Aggregate, AggregateId>
{
    private readonly IDomainEventFactory _domainEventFactory;

    public UpgradeEventToV2(IDomainEventFactory domainEventFactory) => _domainEventFactory = domainEventFactory;

    public IEnumerable<IDomainEvent<Aggregate, AggregateId>> Upgrade(IDomainEvent<Aggregate, AggregateId> domainEvent)
    {
        yield return domainEvent is not IDomainEvent<Aggregate, AggregateId, Event> eventV1
            ? domainEvent
            : _domainEventFactory.Upgrade<Aggregate, AggregateId>(domainEvent,
                new EventV2(eventV1.AggregateEvent.MagicNumber));
    }
}