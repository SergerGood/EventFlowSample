using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;

namespace GettingStartedTest
{
    public class Aggregate : AggregateRoot<Aggregate, AggregateId>, IEmit<Event>
    {
        private int? magicNumber;

        public Aggregate(AggregateId id)
            : base(id)
        {
        }

        public void Apply(Event aggregateEvent)
        {
            magicNumber = aggregateEvent.MagicNumber;
        }

        public IExecutionResult SetMagicNumer(int magicNumber)
        {
            if (this.magicNumber.HasValue)
            {
                return ExecutionResult.Failed("Magic number already set");
            }

            Emit(new Event(magicNumber));

            return ExecutionResult.Success();
        }
    }
}