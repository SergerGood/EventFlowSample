using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;

namespace GettingStartedTest.Model
{
    public class Aggregate : AggregateRoot<Aggregate, AggregateId> //, IEmit<Event>
    {
        private int? magicNumber;

        public Aggregate(AggregateId id)
            : base(id)
        {
            //Register<Event>(Apply);
        }

        private void Apply(Event aggregateEvent)
        {
            magicNumber = aggregateEvent.MagicNumber;
        }

        public IExecutionResult SetMagicNumber(int value)
        {
            if (magicNumber.HasValue)
            {
                return ExecutionResult.Failed("Magic number already set");
            }

            Emit(new Event(value));

            return ExecutionResult.Success();
        }
    }
}