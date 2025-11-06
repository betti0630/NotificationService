using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using MimeKit;

using Notification.Application.Contracts;

namespace Notification.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly string _credentialPath;
    private readonly string _tokenPath;

    public EmailService(string credentialsPath, string fromEmail, string tokenPath = "token.json")
    {
        _fromEmail = fromEmail;
        _credentialPath = credentialsPath;
        _tokenPath = tokenPath;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { GmailService.Scope.GmailSend },
            "user",
            CancellationToken.None,
            new FileDataStore(_tokenPath, true)
        );

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
            HttpClientInitializer = credential,
            ApplicationName = "NotificationService",
        });
        await gmailService.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
    }
}