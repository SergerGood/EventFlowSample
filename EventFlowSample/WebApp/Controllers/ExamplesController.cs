using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Commands;
using EventFlow;
using EventFlow.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExamplesController : Controller
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;


    public ExamplesController(
        ICommandBus commandBus,
        IQueryProcessor queryProcessor)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AggregateReadModel>> GetExample(string id, CancellationToken cancellationToken)
    {
        var readModelByIdQuery = new ReadModelByIdQuery<AggregateReadModel>(id);
        var readModel = await _queryProcessor.ProcessAsync(readModelByIdQuery, cancellationToken);

        return Ok(readModel);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] int value, CancellationToken cancellationToken)
    {
        var exampleCommand = new SetMagicNumberCommand(AggregateId.New, value);

        await _commandBus.PublishAsync(exampleCommand, cancellationToken);

        var routeValues = new
        {
            id = exampleCommand.AggregateId.Value
        };
        
        return CreatedAtAction(nameof(GetExample), routeValues, exampleCommand);
    }
}