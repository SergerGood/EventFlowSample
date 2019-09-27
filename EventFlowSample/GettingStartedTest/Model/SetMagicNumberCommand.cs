using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace GettingStartedTest.Model
{
    public class SetMagicNumberCommand
        : Command<Aggregate, AggregateId, IExecutionResult>
    {
        public SetMagicNumberCommand(AggregateId aggregateId, int magicNumber)
            : base(aggregateId)
        {
            MagicNumber = magicNumber;
        }

        public int MagicNumber { get; }
    }
}