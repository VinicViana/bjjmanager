using BJJManager.Domain.Common;
using BJJManager.Domain.Enums;

namespace BJJManager.Domain.Entities;

public class TechniqueMedia : Entity
{
    public Guid TechniqueId { get; private set; }
    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public MediaType MediaType { get; private set; }

    public TechniqueMedia(Guid techniqueId, string fileName, string fileUrl, MediaType mediaType)
        : base(Guid.NewGuid())
    {
        TechniqueId = techniqueId;
        FileName = fileName;
        FileUrl = fileUrl;
        MediaType = mediaType;
    }
}
