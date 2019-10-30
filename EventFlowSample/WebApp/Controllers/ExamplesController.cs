using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Commands;
using EventFlow;
using EventFlow.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamplesController : Controller
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryProcessor queryProcessor;


        public ExamplesController(
            ICommandBus commandBus,
            IQueryProcessor queryProcessor)
        {
            this.commandBus = commandBus;
            this.queryProcessor = queryProcessor;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AggregateReadModel>> GetExample(string id)
        {
            var readModel = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<AggregateReadModel>(id),
                CancellationToken.None);

            return Ok(readModel);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] int value)
        {
            var exampleCommand = new SetMagicNumberCommand(AggregateId.New, value);

            await commandBus.PublishAsync(exampleCommand, CancellationToken.None);

            return CreatedAtAction(nameof(GetExample), new {id = exampleCommand.AggregateId.Value}, exampleCommand);
        }
    }
}