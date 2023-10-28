using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using EventFlow.ReadStores;

namespace DomainModel.Queries;

public class
    GetMagicNumberByAggregateQueryHandler<TReadStore, TReadModel> : IQueryHandler<GetMagicNumberByAggregateQuery, int>
    where TReadStore : IReadModelStore<AggregateReadModel>
    where TReadModel : AggregateReadModel
{
    private readonly TReadStore _readStore;

    public GetMagicNumberByAggregateQueryHandler(
        TReadStore readStore) =>
        _readStore = readStore;

    public async Task<int> ExecuteQueryAsync(GetMagicNumberByAggregateQuery query,
        CancellationToken cancellationToken)
    {
        var readModelEnvelope = await _readStore.GetAsync(query.Id, cancellationToken).ConfigureAwait(false);
        return readModelEnvelope.ReadModel.MagicNumber;
    }
}