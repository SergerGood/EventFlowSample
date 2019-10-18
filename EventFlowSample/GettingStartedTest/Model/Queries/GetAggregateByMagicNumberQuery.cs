using EventFlow.Queries;

namespace GettingStartedTest.Model.Queries
{
    public class GetAggregateByMagicNumberQuery : IQuery<AggregateReadModel>
    {
        public GetAggregateByMagicNumberQuery(int magicNumber)
        {
            MagicNumber = magicNumber;
        }

        public int MagicNumber { get; }
    }
}