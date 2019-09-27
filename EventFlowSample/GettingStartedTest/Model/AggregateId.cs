using EventFlow.Core;

namespace GettingStartedTest.Model
{
    public class AggregateId : Identity<AggregateId>
    {
        public AggregateId(string value)
            : base(value)
        {
        }
    }
}