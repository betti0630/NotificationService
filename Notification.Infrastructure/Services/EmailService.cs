using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using MimeKit;

using Notification.Application.Contracts;

using System.Net;

namespace Notification.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly UserCredential? _credential;

    public EmailService(string credentialsPath, string fromEmail, string tokenPath = "token.json")
    {
        _fromEmail = fromEmail;

        using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
        _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { GmailService.Scope.GmailSend },
            "user",
            CancellationToken.None,
            new FileDataStore(tokenPath, true)
        ).Result;


       
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var memoryStream = new MemoryStream();
        await message.WriteToAsync(memoryStream);
        var base64UrlEncodedEmail = Convert.ToBase64String(memoryStream.ToArray())
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .Replace("=", "", StringComparison.Ordinal);

        var gmailMessage = new Message { Raw = base64UrlEncodedEmail };
        using var gmailService = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = _credential,
            ApplicationName = "NotificationService",
        });
        await gmailService.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
    }
}