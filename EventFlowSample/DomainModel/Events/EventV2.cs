using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace DomainModel.Events;

[EventVersion("example", 2)]
public class EventV2 : AggregateEvent<Aggregate, AggregateId>
{
    public EventV2(int magicNumber) => MagicNumber = magicNumber;

    public int MagicNumber { get; }
}