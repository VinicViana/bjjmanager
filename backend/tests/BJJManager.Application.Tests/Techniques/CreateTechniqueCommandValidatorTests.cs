using BJJManager.Application.Techniques;
using FluentAssertions;
using Xunit;

namespace BJJManager.Application.Tests.Techniques;

public class CreateTechniqueCommandValidatorTests
{
    private readonly CreateTechniqueCommandValidator _validator = new();

    [Fact]
    public void Validate_WithNoSteps_Fails()
    {
        var result = _validator.Validate(new CreateTechniqueCommand("Armbar", "Mount", "Description", Array.Empty<string>()));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithBlankStepDescription_Fails()
    {
        var result = _validator.Validate(new CreateTechniqueCommand("Armbar", "Mount", "Description", new[] { "  " }));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithValidData_Succeeds()
    {
        var result = _validator.Validate(new CreateTechniqueCommand("Armbar", "Mount", "Description", new[] { "Break posture" }));

        result.IsValid.Should().BeTrue();
    }
}
