using BJJManager.Application.Techniques;
using BJJManager.WebApi.Contracts.Common;
using BJJManager.WebApi.Contracts.Techniques;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BJJManager.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/techniques")]
public class TechniquesController : ControllerBase
{
    private readonly ISender _sender;

    public TechniquesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTechniquesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTechniqueByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTechniqueRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTechniqueCommand(request.Name, request.Position, request.Description, request.Steps);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTechniqueRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTechniqueCommand(id, request.Name, request.Position, request.Description, request.Steps);
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteTechniqueCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/media")]
    public async Task<IActionResult> AddMedia(Guid id, AddMediaRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTechniqueMediaCommand(id, request.FileName, request.FileUrl, request.MediaType);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }

    [HttpDelete("{id:guid}/media/{mediaId:guid}")]
    public async Task<IActionResult> RemoveMedia(Guid id, Guid mediaId, CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoveTechniqueMediaCommand(id, mediaId), cancellationToken);
        return NoContent();
    }
}
