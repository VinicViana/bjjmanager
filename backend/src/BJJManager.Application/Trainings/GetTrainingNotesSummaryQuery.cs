using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Common.Models;
using MediatR;

namespace BJJManager.Application.Trainings;

public record GetTrainingNotesSummaryQuery : IRequest<TrainingNotesSummaryDto>;

public record DailyNoteDto(DateOnly Date, double AverageScore, int SessionCount);

public record TrainingNotesSummaryDto(
    IReadOnlyList<DailyNoteDto> DailyNotes,
    double OverallAverage,
    int TotalSessions);

public class GetTrainingNotesSummaryQueryHandler : IRequestHandler<GetTrainingNotesSummaryQuery, TrainingNotesSummaryDto>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTrainingNotesSummaryQueryHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TrainingNotesSummaryDto> Handle(GetTrainingNotesSummaryQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _trainingSessionRepository.GetByUserIdAsync(
            _currentUserService.UserId, new TrainingSessionFilter(null, null, null), cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = today.AddMonths(-1);

        var lastMonthSessions = sessions
            .Where(s => s.TrainingDate >= startDate && s.TrainingDate <= today)
            .ToList();

        var dailyNotes = lastMonthSessions
            .GroupBy(s => s.TrainingDate)
            .Select(g => new DailyNoteDto(g.Key, Math.Round(g.Average(s => (double)s.SelfEvaluation), 2), g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        var overallAverage = lastMonthSessions.Count > 0
            ? Math.Round(lastMonthSessions.Average(s => (double)s.SelfEvaluation), 2)
            : 0;

        return new TrainingNotesSummaryDto(dailyNotes, overallAverage, lastMonthSessions.Count);
    }
}
