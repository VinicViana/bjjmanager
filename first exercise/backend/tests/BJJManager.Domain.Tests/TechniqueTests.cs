using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using BJJManager.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BJJManager.Domain.Tests;

public class TechniqueTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithNoSteps_ThrowsDomainException()
    {
        var act = () => new Technique(UserId, "Armbar", "Mount", "Classic armbar", Array.Empty<string>());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithSteps_AssignsSequentialOrderStartingAtOne()
    {
        var technique = new Technique(UserId, "Armbar", "Mount", "Classic armbar",
            new[] { "Break posture", "Isolate the arm", "Extend the hips" });

        technique.Steps.Select(s => s.Order).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void ReplaceSteps_WithEmptyList_ThrowsDomainException()
    {
        var technique = new Technique(UserId, "Armbar", "Mount", "Classic armbar", new[] { "Break posture" });

        var act = () => technique.ReplaceSteps(Array.Empty<string>());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ReplaceSteps_WithNewList_RenumbersSequentially()
    {
        var technique = new Technique(UserId, "Armbar", "Mount", "Classic armbar",
            new[] { "Break posture", "Isolate the arm" });

        technique.ReplaceSteps(new[] { "Step A", "Step B", "Step C" });

        technique.Steps.Select(s => s.Description).Should().Equal("Step A", "Step B", "Step C");
        technique.Steps.Select(s => s.Order).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void AddMedia_AddsMediaBelongingToTechnique()
    {
        var technique = new Technique(UserId, "Armbar", "Mount", "Classic armbar", new[] { "Break posture" });

        var media = technique.AddMedia("armbar.mp4", "https://example.com/armbar.mp4", MediaType.Video);

        technique.Media.Should().ContainSingle().Which.Should().BeSameAs(media);
        media.TechniqueId.Should().Be(technique.Id);
    }

    [Fact]
    public void RemoveMedia_RemovesOnlyMatchingMedia()
    {
        var technique = new Technique(UserId, "Armbar", "Mount", "Classic armbar", new[] { "Break posture" });
        var media = technique.AddMedia("armbar.mp4", "https://example.com/armbar.mp4", MediaType.Video);
        technique.AddMedia("armbar.jpg", "https://example.com/armbar.jpg", MediaType.Image);

        technique.RemoveMedia(media.Id);

        technique.Media.Should().ContainSingle().Which.FileName.Should().Be("armbar.jpg");
    }
}
