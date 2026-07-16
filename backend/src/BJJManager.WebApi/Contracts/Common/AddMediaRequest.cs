using BJJManager.Domain.Enums;

namespace BJJManager.WebApi.Contracts.Common;

public record AddMediaRequest(string FileName, string FileUrl, MediaType MediaType);
