using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Subscribers;
using GettingStartedTest.Model.Events;

namespace GettingStartedTest.Model.Subscribers
{
    public class AsynchronousSubscriber: ISubscribeAsynchronousTo<Aggregate, AggregateId,Event>
    {
        public Task HandleAsync(IDomainEvent<Aggregate, AggregateId, Event> domainEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"--> Asynchronous {domainEvent.AggregateEvent.MagicNumber}");
            return Task.CompletedTask;
        }
    }
}