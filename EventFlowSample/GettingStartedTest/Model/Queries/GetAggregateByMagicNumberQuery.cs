using EventFlow.Queries;

namespace GettingStartedTest.Model.Queries
{
    public class GetAggregateByMagicNumberQuery : IQuery<ReadModel>
    {
        public GetAggregateByMagicNumberQuery(int magicNumber)
        {
            MagicNumber = magicNumber;
        }

        public int MagicNumber { get; }
    }
}