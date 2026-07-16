using BJJManager.Domain.Common;
using BJJManager.Domain.Enums;

namespace BJJManager.Domain.Entities;

public class TrainingMedia : Entity
{
    public Guid TrainingId { get; private set; }
    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public MediaType MediaType { get; private set; }

    public TrainingMedia(Guid trainingId, string fileName, string fileUrl, MediaType mediaType)
        : base(Guid.NewGuid())
    {
        TrainingId = trainingId;
        FileName = fileName;
        FileUrl = fileUrl;
        MediaType = mediaType;
    }
}
