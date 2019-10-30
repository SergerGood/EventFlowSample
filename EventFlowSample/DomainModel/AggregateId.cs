using EventFlow.Core;

namespace DomainModel
{
    public class AggregateId : Identity<AggregateId>
    {
        public AggregateId(string value)
            : base(value)
        {
        }
    }
}