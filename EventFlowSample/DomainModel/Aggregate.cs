using DomainModel.Events;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Aggregate : SnapshotAggregateRoot<Aggregate, AggregateId, AggregateSnapshot> //, IEmit<Event>
    {
        private int? magicNumber;

        public Aggregate(AggregateId id)
            : base(id, SnapshotEveryFewVersionsStrategy.With(1))
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

        protected override Task<AggregateSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new AggregateSnapshot());
        }

        protected override Task LoadSnapshotAsync(AggregateSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}