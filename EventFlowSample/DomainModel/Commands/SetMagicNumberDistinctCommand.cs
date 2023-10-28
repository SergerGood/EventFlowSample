using System;
using System.Collections.Generic;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace DomainModel.Commands;

public class SetMagicNumberDistinctCommand
    : DistinctCommand<Aggregate, AggregateId, IExecutionResult>
{
    public SetMagicNumberDistinctCommand(AggregateId aggregateId, int magicNumber)
        : base(aggregateId) =>
        MagicNumber = magicNumber;

    public int MagicNumber { get; }

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(MagicNumber);
    }
}