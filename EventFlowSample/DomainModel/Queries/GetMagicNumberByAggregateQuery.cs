using EventFlow.Core;
using EventFlow.Queries;

namespace DomainModel.Queries
{
    public class GetMagicNumberByAggregateQuery : IQuery<int>
    {
        public string Id { get; }

        public GetMagicNumberByAggregateQuery(IIdentity identity)
            : this(identity.Value)
        {
        }

        public GetMagicNumberByAggregateQuery(string id)
        {
            Id = id;
        }
    }
}