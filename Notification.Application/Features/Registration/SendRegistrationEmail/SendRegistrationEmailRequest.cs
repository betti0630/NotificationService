using MediatR;

namespace Notification.Application.Features.Registration.SendRegistrationEmail;

public class SendRegistrationEmailRequest : IRequest<SendRegistrationEmailResponse>
{
    public int UserId { get; set;}
    public string? Token { get; set; }
    public string? VerificationLink { get; set; }
}

public class SendRegistrationEmailResponse
{
    public bool Success { get; set;}
    public string? Message { get; set;}
}
