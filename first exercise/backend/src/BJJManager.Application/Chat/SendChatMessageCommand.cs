using BJJManager.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Chat;

public record SendChatMessageCommand(IReadOnlyList<ChatMessageDto> Messages) : IRequest<ChatMessageDto>;

public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    private static readonly string[] ValidRoles = { "user", "assistant" };

    public SendChatMessageCommandValidator()
    {
        RuleFor(x => x.Messages)
            .NotEmpty()
            .WithMessage("At least one message is required.");

        RuleFor(x => x.Messages)
            .Must(messages => messages.Count > 0 && messages[^1].Role == "user")
            .WithMessage("The last message must be from the user.")
            .When(x => x.Messages.Count > 0);

        RuleForEach(x => x.Messages).ChildRules(message =>
        {
            message.RuleFor(m => m.Role)
                .Must(role => ValidRoles.Contains(role))
                .WithMessage("Role must be 'user' or 'assistant'.");

            message.RuleFor(m => m.Content)
                .NotEmpty()
                .MaximumLength(2000);
        });
    }
}

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, ChatMessageDto>
{
    private const string SystemPrompt =
        "You are a black belt Brazilian Jiu-Jitsu instructor with decades of experience training and " +
        "competing. Your purpose is to help with Brazilian Jiu-Jitsu — techniques, positions, strategy, " +
        "rules, training methodology, competition, belt progression, and related topics.\n\n" +
        "Greetings and small talk (like \"hi\", \"hello\", \"how are you\", \"who are you\") deserve a " +
        "warm, brief, natural reply that welcomes the person and invites them to ask a jiu-jitsu " +
        "question — never the refusal below for these.\n\n" +
        "Only if asked something clearly unrelated to jiu-jitsu (a different subject entirely), politely " +
        "say you can only help with jiu-jitsu topics and steer the conversation back.\n\n" +
        "Always respond in the same language the question was written in, regardless of what language " +
        "this instruction is in.\n\n" +
        "Keep answers complete but simple: cover what the person actually needs to know, in plain " +
        "language, without unnecessary jargon. If you do use a technical term, briefly explain it.";

    private readonly IAiChatClient _aiChatClient;

    public SendChatMessageCommandHandler(IAiChatClient aiChatClient)
    {
        _aiChatClient = aiChatClient;
    }

    public async Task<ChatMessageDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var messagesWithSystemPrompt = new List<ChatMessageDto> { new("system", SystemPrompt) };
        messagesWithSystemPrompt.AddRange(request.Messages);

        var reply = await _aiChatClient.GetReplyAsync(messagesWithSystemPrompt, cancellationToken);

        return new ChatMessageDto("assistant", reply);
    }
}
