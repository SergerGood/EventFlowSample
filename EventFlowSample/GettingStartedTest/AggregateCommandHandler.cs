using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace GettingStartedTest
{
    public class AggregateCommandHandler : CommandHandler<Aggregate, AggregateId, IExecutionResult, AggregateCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(Aggregate aggregate, AggregateCommand command,
            CancellationToken cancellationToken)
        {
            var executionResult = aggregate.SetMagicNumer(command.MagicNumber);

            return Task.FromResult(executionResult);
        }
    }
}