using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;

namespace GettingStartedTest.Model.Queries
{
    public class GetAggregateByMagicNumberQueryHandler : IQueryHandler<GetAggregateByMagicNumberQuery, AggregateReadModel>
    {
        public async Task<AggregateReadModel> ExecuteQueryAsync(GetAggregateByMagicNumberQuery query,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(new AggregateReadModel());
        }
    }
}