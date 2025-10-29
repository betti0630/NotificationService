namespace Notification.Infrastructure.Models;

public class EmailSettings
{
    public string FromEmail { get; set; } = default!;
    public string CredentialPath { get; set; } = default!;
    public string TokenPath { get; set; } = default!;
}
