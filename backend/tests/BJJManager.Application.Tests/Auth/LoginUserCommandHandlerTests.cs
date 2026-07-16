using BJJManager.Application.Auth;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Auth;

public class LoginUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

    private LoginUserCommandHandler CreateHandler() => new(_userRepository, _passwordHasher, _jwtTokenGenerator);

    [Fact]
    public async Task Handle_WithCorrectPassword_ReturnsToken()
    {
        var user = new User("Vinicius", "hashed-secret");
        _userRepository.GetAllByNameAsync("Vinicius", Arg.Any<CancellationToken>())
            .Returns(new[] { user });
        _passwordHasher.Verify("Secret6", "hashed-secret").Returns(true);
        var expiry = DateTime.UtcNow.AddHours(1);
        _jwtTokenGenerator.GenerateToken(user).Returns(("jwt-token", expiry));

        var result = await CreateHandler().Handle(new LoginUserCommand("Vinicius", "Secret6"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.UserName.Should().Be("Vinicius");
    }

    [Fact]
    public async Task Handle_WithNoMatchingUser_ThrowsUnauthorizedAccessException()
    {
        _userRepository.GetAllByNameAsync("Vinicius", Arg.Any<CancellationToken>())
            .Returns(Array.Empty<User>());

        var act = () => CreateHandler().Handle(new LoginUserCommand("Vinicius", "WrongPassword"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User("Vinicius", "hashed-secret");
        _userRepository.GetAllByNameAsync("Vinicius", Arg.Any<CancellationToken>())
            .Returns(new[] { user });
        _passwordHasher.Verify("WrongPassword", "hashed-secret").Returns(false);

        var act = () => CreateHandler().Handle(new LoginUserCommand("Vinicius", "WrongPassword"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
