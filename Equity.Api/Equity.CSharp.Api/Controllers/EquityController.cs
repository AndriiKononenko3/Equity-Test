using Equity.CSharp.Api.Commands;
using Equity.CSharp.Api.Queries;
using Equity.CSharp.Api.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Equity.CSharp.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class EquityController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetAllAsync([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EquityPlanQuery.Request(id), cancellationToken);

        return result.Match(
            success => Results.Ok(success),
            errors => Results.BadRequest(errors)); 
    }
    
    [HttpPost]
    public async Task<IResult> CreateAsync(CreateEquityPlanRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateEquityPlanCommand.Request(request), cancellationToken);

        return result.Match(
            id => Results.Ok(id),
            errors => Results.BadRequest(errors)); 
    }
}