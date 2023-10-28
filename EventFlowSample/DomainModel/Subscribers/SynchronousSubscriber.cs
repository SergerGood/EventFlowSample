using System;
using System.Threading;
using System.Threading.Tasks;
using DomainModel.Events;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DomainModel.Subscribers;

public class SynchronousSubscriber : ISubscribeSynchronousTo<Aggregate, AggregateId, Event>
{
    public Task HandleAsync(IDomainEvent<Aggregate, AggregateId, Event> domainEvent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"--> Synchronous {domainEvent.AggregateEvent.MagicNumber}");

        return Task.CompletedTask;
    }
}