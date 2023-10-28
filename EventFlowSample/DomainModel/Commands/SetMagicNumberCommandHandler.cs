using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace DomainModel.Commands;

public class SetMagicNumberCommandHandler
    : CommandHandler<Aggregate, AggregateId, IExecutionResult, SetMagicNumberCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(Aggregate aggregate, SetMagicNumberCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var executionResult = aggregate.SetMagicNumber(command.MagicNumber);

        return Task.FromResult(executionResult);
    }
}