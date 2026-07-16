using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BJJManager.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BjjManagerDbContext _context;

    public UserRepository(BjjManagerDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<IReadOnlyList<User>> GetAllByNameAsync(string name, CancellationToken cancellationToken) =>
        await _context.Users.Where(u => u.Name == name).ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken) =>
        await _context.Users.AddAsync(user, cancellationToken);
}
