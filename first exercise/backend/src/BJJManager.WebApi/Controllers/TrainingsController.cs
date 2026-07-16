using BJJManager.Application.Trainings;
using BJJManager.WebApi.Contracts.Common;
using BJJManager.WebApi.Contracts.Trainings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BJJManager.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/trainings")]
public class TrainingsController : ControllerBase
{
    private readonly ISender _sender;

    public TrainingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTrainingsQuery(date, month, year), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTrainingByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("notes-summary")]
    public async Task<IActionResult> GetNotesSummary(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTrainingNotesSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTrainingRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTrainingCommand(request.TrainingDate, request.AcademyName, request.SelfEvaluation, request.Notes);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTrainingRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTrainingCommand(id, request.TrainingDate, request.AcademyName, request.SelfEvaluation, request.Notes);
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteTrainingCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/media")]
    public async Task<IActionResult> AddMedia(Guid id, AddMediaRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTrainingMediaCommand(id, request.FileName, request.FileUrl, request.MediaType);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }

    [HttpDelete("{id:guid}/media/{mediaId:guid}")]
    public async Task<IActionResult> RemoveMedia(Guid id, Guid mediaId, CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoveTrainingMediaCommand(id, mediaId), cancellationToken);
        return NoContent();
    }
}
