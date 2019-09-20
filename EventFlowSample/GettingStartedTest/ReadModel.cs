using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace GettingStartedTest
{
    public class ReadModel : IReadModel, IAmReadModelFor<Aggregate, AggregateId, Event>
    {
        public int MagicNumber { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<Aggregate, AggregateId, Event> domainEvent)
        {
            MagicNumber = domainEvent.AggregateEvent.MagicNumber;
        }
    }
}