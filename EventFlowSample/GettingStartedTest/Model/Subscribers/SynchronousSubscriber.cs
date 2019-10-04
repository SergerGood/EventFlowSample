using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace GettingStartedTest.Model.Subscribers
{
    public class SynchronousSubscriber: ISubscribeSynchronousTo<Aggregate, AggregateId,Event>
    {
        public Task HandleAsync(IDomainEvent<Aggregate, AggregateId, Event> domainEvent, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}