using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Commands
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