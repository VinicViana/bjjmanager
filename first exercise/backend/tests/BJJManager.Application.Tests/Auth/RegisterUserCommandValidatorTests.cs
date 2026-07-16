using BJJManager.Application.Auth;
using FluentAssertions;
using Xunit;

namespace BJJManager.Application.Tests.Auth;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithPasswordShorterThanSixCharacters_Fails()
    {
        var result = _validator.Validate(new RegisterUserCommand("Vinicius", "abc12"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyName_Fails()
    {
        var result = _validator.Validate(new RegisterUserCommand(string.Empty, "abcdef"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithValidNameAndPassword_Succeeds()
    {
        var result = _validator.Validate(new RegisterUserCommand("Vinicius", "abcdef"));

        result.IsValid.Should().BeTrue();
    }
}
