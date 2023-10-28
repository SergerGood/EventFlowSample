using EventFlow.Core;
using EventFlow.Queries;

namespace DomainModel.Queries;

public class GetMagicNumberByAggregateQuery : IQuery<int>
{
    public GetMagicNumberByAggregateQuery(IIdentity identity)
        : this(identity.Value)
    {
    }

    public GetMagicNumberByAggregateQuery(string id) => Id = id;

    public string Id { get; }
}