using EventFlow.Core;

namespace GettingStartedTest
{
    public class AggregateId : Identity<AggregateId>
    {
        public AggregateId(string value)
            : base(value)
        {
        }
    }
}