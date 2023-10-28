using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace DomainModel.Commands;

public class SetMagicNumberDistinctCommandHandler
    : CommandHandler<Aggregate, AggregateId, IExecutionResult, SetMagicNumberDistinctCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(Aggregate aggregate,
        SetMagicNumberDistinctCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var executionResult = aggregate.SetMagicNumber(command.MagicNumber);

        return Task.FromResult(executionResult);
    }
}