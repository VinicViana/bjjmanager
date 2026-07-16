using BJJManager.Domain.Enums;

namespace BJJManager.WebApi.Contracts.Trainings;

public record UpdateTrainingRequest(DateOnly TrainingDate, string AcademyName, SelfEvaluation SelfEvaluation, string? Notes);
