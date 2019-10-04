using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace GettingStartedTest.Model.Commands
{
    public class SetMagicNumberDistinctCommandHandler
        : CommandHandler<Aggregate, AggregateId, IExecutionResult, SetMagicNumberDistinctCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(Aggregate aggregate, SetMagicNumberDistinctCommand command,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IExecutionResult executionResult = aggregate.SetMagicNumber(command.MagicNumber);

            return Task.FromResult(executionResult);
        }
    }
}