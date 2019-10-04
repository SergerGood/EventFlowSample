using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Exceptions;
using EventFlow.Extensions;
using EventFlow.Queries;
using FluentAssertions;
using GettingStartedTest.Model;
using NUnit.Framework;

namespace GettingStartedTest
{
    public class ExampleTests
    {
        [Test]
        public async Task ShouldHandleCommand()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                using (var resolver = EventFlowOptions.New
                    .AddEvents(typeof(Event))
                    .AddCommands(typeof(SetMagicNumberCommand))
                    .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                    .UseInMemoryReadStoreFor<ReadModel>()
                    .CreateResolver())
                {
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
            }
        }

        [Test]
        public async Task ShouldThrowDuplicateOperationException()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                using (var resolver = EventFlowOptions.New
                    .AddEvents(typeof(Event))
                    .AddCommands(typeof(SetMagicNumberDistinctCommand))
                    .AddCommandHandlers(typeof(SetMagicNumberDistinctCommandHandler))
                    .UseInMemoryReadStoreFor<ReadModel>()
                    .CreateResolver())
                {
                    var exampleId = AggregateId.NewComb();
                    const int magicNumber = 42;

                    var commandBus = resolver.Resolve<ICommandBus>();

                    var command = new SetMagicNumberDistinctCommand(exampleId, magicNumber);
                    await commandBus.PublishAsync(command, tokenSource.Token);

                    var command2 = new SetMagicNumberDistinctCommand(exampleId, magicNumber);

                    Assert.ThrowsAsync<DuplicateOperationException>(async () =>
                        await commandBus.PublishAsync(command2, tokenSource.Token));
                }
            }
        }
    }
}