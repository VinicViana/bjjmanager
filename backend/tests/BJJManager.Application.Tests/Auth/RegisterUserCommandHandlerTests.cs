using BJJManager.Application.Auth;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Auth;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private RegisterUserCommandHandler CreateHandler() => new(_userRepository, _passwordHasher, _unitOfWork);

    [Fact]
    public async Task Handle_HashesPasswordAndPersistsUser()
    {
        _passwordHasher.Hash("Secret6").Returns("hashed-secret");

        var result = await CreateHandler().Handle(new RegisterUserCommand("Vinicius", "Secret6"), CancellationToken.None);

        result.Name.Should().Be("Vinicius");
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Name == "Vinicius" && u.PasswordHash == "hashed-secret"),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
