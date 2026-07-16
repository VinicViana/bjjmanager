using BJJManager.Domain.Entities;

namespace BJJManager.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetAllByNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}
