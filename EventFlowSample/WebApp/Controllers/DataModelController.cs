using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using EventFlow.Configuration;
using EventFlow.ReadStores;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataModelController : Controller
{
    private readonly IReadModelPopulator _readModelPopulator;

    public DataModelController(IResolver resolver)
    {
        _readModelPopulator = resolver.Resolve<IReadModelPopulator>();
    }

    [HttpPost]
    public async Task<ActionResult> ReplayEvents(CancellationToken cancellationToken)
    {
        await _readModelPopulator.PopulateAsync<AggregateReadModel>(cancellationToken);

        return Accepted("Read models are replayed");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteEvents(CancellationToken cancellationToken)
    {
        await _readModelPopulator.PurgeAsync<AggregateReadModel>(cancellationToken);

        return Ok("Read models deleted");
    }
}