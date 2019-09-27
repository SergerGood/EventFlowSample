using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Configuration;
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
        public async Task GettingStartedExample()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                using (IRootResolver resolver = EventFlowOptions.New
                    .AddEvents(typeof(Event))
                    .AddCommands(typeof(SetMagicNumberCommand))
                    .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                    .UseInMemoryReadStoreFor<ReadModel>()
                    .CreateResolver())
                {
                    AggregateId exampleId = AggregateId.NewComb();
                    const int magicNumber = 42;

                    var commandBus = resolver.Resolve<ICommandBus>();

                    var command = new SetMagicNumberCommand(exampleId, magicNumber);
                    IExecutionResult executionResult1 = await commandBus.PublishAsync(command, tokenSource.Token);

                    var command2 = new SetMagicNumberCommand(exampleId, magicNumber);
                    IExecutionResult executionResult2 = await commandBus.PublishAsync(command2, tokenSource.Token);

                    executionResult1.IsSuccess.Should().BeTrue();
                    executionResult2.IsSuccess.Should().BeFalse();

                    var queryProcessor = resolver.Resolve<IQueryProcessor>();
                    
                    var query = new ReadModelByIdQuery<ReadModel>(exampleId);
                    ReadModel readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

                    readModel.MagicNumber.Should().Be(magicNumber);
                }
            }
        }
    }
}