using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BJJManager.Infrastructure.Identity;

public class EfPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string Hash(string password) => _passwordHasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash) =>
        _passwordHasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
