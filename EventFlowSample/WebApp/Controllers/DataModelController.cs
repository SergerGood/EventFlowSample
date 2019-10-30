using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using EventFlow.Configuration;
using EventFlow.ReadStores;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataModelController : Controller
    {
        private readonly IReadModelPopulator readModelPopulator;

        public DataModelController(IResolver resolver)
        {
            readModelPopulator = resolver.Resolve<IReadModelPopulator>();
        }

        [HttpPost]
        public async Task<ActionResult> ReplayEvents()
        {
            await readModelPopulator.PopulateAsync<AggregateReadModel>(CancellationToken.None);

            return Accepted("Read models are replayed");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteEvents()
        {
            await readModelPopulator.PurgeAsync<AggregateReadModel>(CancellationToken.None);

            return Ok("Read models deleted");
        }
    }
}