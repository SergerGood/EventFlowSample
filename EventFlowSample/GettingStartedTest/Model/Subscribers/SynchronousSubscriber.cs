using System;
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
            Console.WriteLine($"--> Synchronous {domainEvent.AggregateEvent.MagicNumber}");

            return Task.CompletedTask;
        }
    }
}