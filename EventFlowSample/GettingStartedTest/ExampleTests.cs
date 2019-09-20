using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.Queries;
using FluentAssertions;
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
                using (var resolver = EventFlowOptions.New
                    .AddEvents(typeof(Event))
                    .AddCommands(typeof(AggregateCommand))
                    .AddCommandHandlers(typeof(AggregateCommandHandler))
                    .UseInMemoryReadStoreFor<ReadModel>()
                    .CreateResolver())
                {
                    var exampleId = AggregateId.New;
                    const int magicNumber = 42;

                    var commandBus = resolver.Resolve<ICommandBus>();
                    var command = new AggregateCommand(exampleId, magicNumber);

                    var executionResult = await commandBus.PublishAsync(command, tokenSource.Token);

                    executionResult.IsSuccess.Should().BeTrue();


                    var queryProcessor = resolver.Resolve<IQueryProcessor>();
                    var query = new ReadModelByIdQuery<ReadModel>(exampleId);

                    ReadModel readModel = await queryProcessor.ProcessAsync(query, tokenSource.Token);

                    readModel.MagicNumber.Should().Be(magicNumber);
                }
            }
        }
    }
}