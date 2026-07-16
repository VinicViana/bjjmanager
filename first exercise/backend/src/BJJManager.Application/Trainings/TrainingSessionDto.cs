using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;

namespace BJJManager.Application.Trainings;

public record TrainingMediaDto(Guid Id, string FileName, string FileUrl, MediaType MediaType);

public record TrainingSessionDto(
    Guid Id,
    DateOnly TrainingDate,
    string AcademyName,
    string? Notes,
    SelfEvaluation SelfEvaluation,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<TrainingMediaDto> Media);

public static class TrainingSessionMappings
{
    public static TrainingSessionDto ToDto(this TrainingSession session) =>
        new(
            session.Id,
            session.TrainingDate,
            session.AcademyName,
            session.Notes,
            session.SelfEvaluation,
            session.CreatedAtUtc,
            session.Media.Select(m => m.ToDto()).ToList());

    public static TrainingMediaDto ToDto(this TrainingMedia media) =>
        new(media.Id, media.FileName, media.FileUrl, media.MediaType);
}
