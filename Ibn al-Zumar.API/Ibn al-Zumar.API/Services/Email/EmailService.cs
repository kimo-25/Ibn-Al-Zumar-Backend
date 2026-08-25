using System.Text.Json;
using IbnAlZumar.Api.Common.Settings;
using Microsoft.Extensions.Options;
using RestSharp;

namespace IbnAlZumar.Api.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string htmlContent)
    {
        var client = new RestClient("https://api.brevo.com/v3/smtp/email");

        var request = new RestRequest();

        request.Method = Method.Post;

        request.AddHeader("accept", "application/json");

        request.AddHeader(
            "api-key",
            _settings.ApiKey);

        request.AddHeader(
            "content-type",
            "application/json");

        var body = new
        {
            sender = new
            {
                name = "Ibn Al Zumar",
                email = "kimo34443@gmail.com"
            },
            to = new[]
            {
                new
                {
                    email = to
                }
            },
            subject,
            htmlContent
        };

        request.AddStringBody(
            JsonSerializer.Serialize(body),
            DataFormat.Json);

        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception(
                $"StatusCode: {response.StatusCode}\n" +
                $"Content: {response.Content}\n" +
                $"ErrorMessage: {response.ErrorMessage}");
        }
    }
}