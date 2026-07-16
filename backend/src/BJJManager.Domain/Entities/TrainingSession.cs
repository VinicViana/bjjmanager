using BJJManager.Domain.Common;
using BJJManager.Domain.Enums;

namespace BJJManager.Domain.Entities;

public class TrainingSession : Entity
{
    private readonly List<TrainingMedia> _media = new();

    public Guid UserId { get; private set; }
    public DateOnly TrainingDate { get; private set; }
    public string AcademyName { get; private set; }
    public string? Notes { get; private set; }
    public SelfEvaluation SelfEvaluation { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<TrainingMedia> Media => _media.AsReadOnly();

    public TrainingSession(
        Guid userId,
        DateOnly trainingDate,
        string academyName,
        SelfEvaluation selfEvaluation,
        string? notes)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TrainingDate = trainingDate;
        AcademyName = academyName;
        SelfEvaluation = selfEvaluation;
        Notes = notes;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(DateOnly trainingDate, string academyName, SelfEvaluation selfEvaluation, string? notes)
    {
        TrainingDate = trainingDate;
        AcademyName = academyName;
        SelfEvaluation = selfEvaluation;
        Notes = notes;
    }

    public TrainingMedia AddMedia(string fileName, string fileUrl, MediaType mediaType)
    {
        var media = new TrainingMedia(Id, fileName, fileUrl, mediaType);
        _media.Add(media);
        return media;
    }

    public void RemoveMedia(Guid mediaId)
    {
        _media.RemoveAll(m => m.Id == mediaId);
    }
}
