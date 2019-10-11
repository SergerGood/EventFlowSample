using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Exceptions;
using EventFlow.Extensions;
using EventFlow.MetadataProviders;
using EventFlow.Queries;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using FluentAssertions;
using GettingStartedTest.Model;
using GettingStartedTest.Model.Commands;
using GettingStartedTest.Model.Subscribers;
using NUnit.Framework;

namespace GettingStartedTest
{
    public class ExampleTests
    {
        [Test]
        public async Task ShouldHandleCommand()
        {
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .UseInMemoryReadStoreFor<ReadModel>()
                .CreateResolver();

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberCommand(exampleId, magicNumber);
            var executionResult1 = await commandBus.PublishAsync(command, tokenSource.Token);

            var command2 = new SetMagicNumberCommand(exampleId, magicNumber);
            var executionResult2 = await commandBus.PublishAsync(command2, tokenSource.Token);

            executionResult1.IsSuccess.Should().BeTrue();
            executionResult2.IsSuccess.Should().BeFalse();

            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            var query = new ReadModelByIdQuery<ReadModel>(exampleId);
            var readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

            readModel.MagicNumber.Should().Be(magicNumber);
        }

        [Test]
        public async Task ShouldThrowDuplicateOperationException()
        {
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberDistinctCommand))
                .AddCommandHandlers(typeof(SetMagicNumberDistinctCommandHandler))
                .CreateResolver();

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberDistinctCommand(exampleId, magicNumber);
            await commandBus.PublishAsync(command, tokenSource.Token);

            var command2 = new SetMagicNumberDistinctCommand(exampleId, magicNumber);

            Assert.ThrowsAsync<DuplicateOperationException>(async () =>
                await commandBus.PublishAsync(command2, tokenSource.Token));
        }

        [Test]
        public async Task ShouldHandlePublishToRabbitMq()
        {
            var stopwatch = new Stopwatch();
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .Configure(x =>
                {
                    x.IsAsynchronousSubscribersEnabled = true;
                    x.ThrowSubscriberExceptions = true;
                })
                .AddMetadataProvider<AddMachineNameMetadataProvider>()
                .PublishToRabbitMq(RabbitMqConfiguration.With(new Uri("amqp://localhost:5672")))
                .AddSynchronousSubscriber<Aggregate, AggregateId, Event, SynchronousSubscriber>()
                .AddAsynchronousSubscriber<Aggregate, AggregateId, Event, AsynchronousSubscriber>()
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .CreateResolver();

            var exampleId = AggregateId.NewComb();

            var commandBus = resolver.Resolve<ICommandBus>();

            var tasks = Enumerable.Range(0, 100)
                .Select(i =>
                {
                    var command = new SetMagicNumberCommand(exampleId, i);
                    return commandBus.PublishAsync(command, tokenSource.Token);
                });

            stopwatch.Start();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}");
        }
    }
}