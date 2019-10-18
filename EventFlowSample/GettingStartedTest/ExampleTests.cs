using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Core;
using EventFlow.Exceptions;
using EventFlow.Extensions;
using EventFlow.MetadataProviders;
using EventFlow.PostgreSql;
using EventFlow.PostgreSql.Connections;
using EventFlow.PostgreSql.EventStores;
using EventFlow.PostgreSql.Extensions;
using EventFlow.Queries;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using FluentAssertions;
using GettingStartedTest.Model;
using GettingStartedTest.Model.Commands;
using GettingStartedTest.Model.Events;
using GettingStartedTest.Model.Queries;
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
                .UseInMemoryReadStoreFor<AggregateReadModel>()
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

            var query = new ReadModelByIdQuery<AggregateReadModel>(exampleId);
            AggregateReadModel readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

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

            var stopwatch = Stopwatch.StartNew();

            var tasks = Enumerable.Range(0, 100)
                .Select(i =>
                {
                    var command = new SetMagicNumberCommand(exampleId, i);
                    return commandBus.PublishAsync(command, tokenSource.Token);
                });

            await Task.WhenAll(tasks);

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}");
        }

        [Test]
        public async Task ShouldHandleQuery()
        {
            var containerBuilder = new ContainerBuilder();
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .UseAutofacContainerBuilder(containerBuilder)
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .UseInMemoryReadStoreFor<AggregateReadModel>()
                .AddQueryHandler<GetAggregateByMagicNumberQueryHandler, GetAggregateByMagicNumberQuery, AggregateReadModel>()
                .CreateResolver();

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberCommand(exampleId, magicNumber);
            await commandBus.PublishAsync(command, tokenSource.Token);


            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            var query = new GetAggregateByMagicNumberQuery(magicNumber);
            AggregateReadModel readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

            readModel.MagicNumber.Should().Be(magicNumber);
        }

        [Test]
        public async Task ShouldEventUpgraded()
        {
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .UseInMemoryReadStoreFor<AggregateReadModel>()
                .AddEventUpgrader<Aggregate, AggregateId, UpgradeEventToV2>()
                .CreateResolver();

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberCommand(exampleId, magicNumber);
            await commandBus.PublishAsync(command, tokenSource.Token);


            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            var query = new ReadModelByIdQuery<AggregateReadModel>(exampleId);
            AggregateReadModel readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

            readModel.MagicNumber.Should().Be(magicNumber);
        }

        [Test]
        public async Task ShouldUsePostgres()
        {
            var connectionString = @"Server=localhost;Port=5432;Database=event_flow_sample;User id=postgres;password=postgres;";
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .Configure(configuration =>
                {
                    configuration.DelayBeforeRetryOnOptimisticConcurrencyExceptions = TimeSpan.FromSeconds(5);
                    configuration.NumberOfRetriesOnOptimisticConcurrencyExceptions = 5;
                    configuration.PopulateReadModelEventPageSize = 200;
                })
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .ConfigurePostgreSql(PostgreSqlConfiguration.New
                    .SetConnectionString(connectionString)
                    .SetTransientRetryDelay(RetryDelay.Between(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)))
                    .SetTransientRetryCount(5))
                .UsePostgreSqlEventStore()
                .UsePostgreSqlReadModel<AggregateReadModel>()
                .CreateResolver();

            var databaseMigrator = resolver.Resolve<IPostgreSqlDatabaseMigrator>();
            EventFlowEventStoresPostgreSql.MigrateDatabase(databaseMigrator);

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberCommand(exampleId, magicNumber);
            await commandBus.PublishAsync(command, tokenSource.Token);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            var query = new ReadModelByIdQuery<AggregateReadModel>(exampleId);
            var readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

            readModel.MagicNumber.Should().Be(magicNumber);
        }

        [Test]
        public async Task ShouldUseSnapshots()
        {
            var connectionString = @"Server=localhost;Port=5432;Database=event_flow_sample;User id=postgres;password=postgres;";
            using var tokenSource = new CancellationTokenSource();

            using var resolver = EventFlowOptions.New
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .ConfigurePostgreSql(PostgreSqlConfiguration.New
                    .SetConnectionString(connectionString))
                .UsePostgreSqlEventStore()
                .UsePostgreSqlReadModel<AggregateReadModel>()
                .AddSnapshots(typeof(AggregateSnapshot))
                .UsePostgreSqlSnapshotStore()
                .CreateResolver();

            var databaseMigrator = resolver.Resolve<IPostgreSqlDatabaseMigrator>();
            EventFlowEventStoresPostgreSql.MigrateDatabase(databaseMigrator);

            var exampleId = AggregateId.NewComb();
            const int magicNumber = 42;

            var commandBus = resolver.Resolve<ICommandBus>();

            var command = new SetMagicNumberCommand(exampleId, magicNumber);
            await commandBus.PublishAsync(command, tokenSource.Token);

            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            var query = new ReadModelByIdQuery<AggregateReadModel>(exampleId);
            var readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

            readModel.MagicNumber.Should().Be(magicNumber);
        }
    }
}