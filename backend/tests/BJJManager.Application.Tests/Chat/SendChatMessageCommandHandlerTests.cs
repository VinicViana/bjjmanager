using BJJManager.Application.Chat;
using BJJManager.Application.Common.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Chat;

public class SendChatMessageCommandHandlerTests
{
    private readonly IAiChatClient _aiChatClient = Substitute.For<IAiChatClient>();

    private SendChatMessageCommandHandler CreateHandler() => new(_aiChatClient);

    [Fact]
    public async Task Handle_ReturnsAssistantReplyFromAiChatClient()
    {
        _aiChatClient
            .GetReplyAsync(Arg.Any<IReadOnlyList<ChatMessageDto>>(), Arg.Any<CancellationToken>())
            .Returns("Break your opponent's posture before attempting the armbar.");

        var command = new SendChatMessageCommand(new[] { new ChatMessageDto("user", "How do I finish an armbar?") });

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.Role.Should().Be("assistant");
        result.Content.Should().Be("Break your opponent's posture before attempting the armbar.");
    }

    [Fact]
    public async Task Handle_PrependsSystemPromptAheadOfConversation()
    {
        _aiChatClient
            .GetReplyAsync(Arg.Any<IReadOnlyList<ChatMessageDto>>(), Arg.Any<CancellationToken>())
            .Returns("reply");

        var command = new SendChatMessageCommand(new[] { new ChatMessageDto("user", "What is a triangle choke?") });

        await CreateHandler().Handle(command, CancellationToken.None);

        await _aiChatClient.Received(1).GetReplyAsync(
            Arg.Is<IReadOnlyList<ChatMessageDto>>(messages =>
                messages.Count == 2 &&
                messages[0].Role == "system" &&
                messages[0].Content.Contains("black belt") &&
                messages[1].Role == "user" &&
                messages[1].Content == "What is a triangle choke?"),
            Arg.Any<CancellationToken>());
    }
}
