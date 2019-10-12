using EventFlow.Aggregates;
using EventFlow.ReadStores;
using GettingStartedTest.Model.Events;

namespace GettingStartedTest.Model
{
    public class ReadModel : IReadModel, IAmReadModelFor<Aggregate, AggregateId, Event>
    {
        public int MagicNumber { get; private set; }

        public void Apply(IReadModelContext context,
            IDomainEvent<Aggregate, AggregateId, Event> domainEvent)
        {
            MagicNumber = domainEvent.AggregateEvent.MagicNumber;
        }
    }
}