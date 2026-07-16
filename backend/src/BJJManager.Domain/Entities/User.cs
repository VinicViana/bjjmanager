using BJJManager.Domain.Common;

namespace BJJManager.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public User(string name, string passwordHash)
        : base(Guid.NewGuid())
    {
        Name = name;
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
