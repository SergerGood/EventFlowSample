using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace DomainModel.Commands;

public class SetMagicNumberCommand
    : Command<Aggregate, AggregateId, IExecutionResult>
{
    public SetMagicNumberCommand(AggregateId aggregateId, int magicNumber)
        : base(aggregateId) =>
        MagicNumber = magicNumber;

    public int MagicNumber { get; }
}