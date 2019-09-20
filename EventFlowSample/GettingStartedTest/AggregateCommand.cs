using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace GettingStartedTest
{
    public class AggregateCommand : Command<Aggregate, AggregateId, IExecutionResult>
    {
        public AggregateCommand(AggregateId aggregateId, int magicNumber)
            : base(aggregateId)
        {
            MagicNumber = magicNumber;
        }

        public int MagicNumber { get; }
    }
}