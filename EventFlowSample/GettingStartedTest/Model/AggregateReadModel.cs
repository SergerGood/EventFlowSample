using EventFlow.Aggregates;
using EventFlow.PostgreSql.ReadStores.Attributes;
using EventFlow.ReadStores;
using GettingStartedTest.Model.Events;

namespace GettingStartedTest.Model
{
    public class AggregateReadModel : IReadModel, IAmReadModelFor<Aggregate, AggregateId, Event>
    {
        [PostgreSqlReadModelIdentityColumn]
        public string Id { get; set; }

        public int MagicNumber { get; set; }

        public void Apply(IReadModelContext context,
            IDomainEvent<Aggregate, AggregateId, Event> domainEvent)
        {
            Id = domainEvent.AggregateIdentity.Value;

            MagicNumber = domainEvent.AggregateEvent.MagicNumber;
        }
    }
}