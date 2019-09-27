using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace GettingStartedTest.Model
{
    [EventVersion("example", 1)]
    public class Event : AggregateEvent<Aggregate, AggregateId>
    {
        public Event(int magicNumber)
        {
            MagicNumber = magicNumber;
        }

        public int MagicNumber { get; }
    }
}