using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BJJManager.Application.Chat;
using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BJJManager.Infrastructure.Ai;

public class OpenAiChatClient : IAiChatClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;

    public OpenAiChatClient(HttpClient httpClient, IOptions<OpenAiSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    public async Task<string> GetReplyAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken)
    {
        var request = new OpenAiChatRequest(
            _settings.Model,
            messages.Select(m => new OpenAiChatRequestMessage(m.Role, m.Content)).ToList());

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsJsonAsync("chat/completions", request, JsonOptions, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new ExternalServiceException("Failed to reach the AI service.", exception);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ExternalServiceException("The AI service timed out.", exception);
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"The AI service returned an error ({(int)response.StatusCode}).");

        var payload = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>(JsonOptions, cancellationToken);
        var reply = payload?.Choices.FirstOrDefault()?.Message.Content;

        if (string.IsNullOrWhiteSpace(reply))
            throw new ExternalServiceException("The AI service returned an empty response.");

        return reply;
    }

    private record OpenAiChatRequest(string Model, IReadOnlyList<OpenAiChatRequestMessage> Messages);

    private record OpenAiChatRequestMessage(string Role, string Content);

    private record OpenAiChatResponse(IReadOnlyList<OpenAiChatChoice> Choices);

    private record OpenAiChatChoice(OpenAiChatResponseMessage Message);

    private record OpenAiChatResponseMessage(string Content);
}
