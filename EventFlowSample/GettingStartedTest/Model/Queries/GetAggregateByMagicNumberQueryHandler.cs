using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;

namespace GettingStartedTest.Model.Queries
{
    public class GetAggregateByMagicNumberQueryHandler : IQueryHandler<GetAggregateByMagicNumberQuery, ReadModel>
    {
        public async Task<ReadModel> ExecuteQueryAsync(GetAggregateByMagicNumberQuery query,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ReadModel());
        }
    }
}