using Grpc.Common.Notification;
using Notification.Application.Contracts;
using Notification.Application.Features.Registration.SendRegistrationEmail;

using Grpc.Core;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Notification.Infrastructure.Services;

public class NotificationService : Grpc.Common.Notification.NotificationService.NotificationServiceBase
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IMediator _mediator;

    public NotificationService(ILogger<NotificationService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task<NotificationResponse> SendRegistrationEmail(RegistrationEmailRequest request, ServerCallContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        var appRequest = new SendRegistrationEmailRequest
        {
            UserId = request.UserId,
            Token = request.Token,
            VerificationLink = request.VerificationLink,
        };
        var response = await _mediator.Send(appRequest);

        return new NotificationResponse
        {
            Success = response.Success,
            Message = response.Message
        };
    }

    public override Task<NotificationResponse> SendFeedNotification(FeedNotificationRequest request, ServerCallContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        //_logger.LogInformation($"[Notification] {request.Type} feedId={request.FeedId} from={request.FromUserId} to={request.ToUserId}");

        return Task.FromResult(new NotificationResponse
        {
            Success = true,
            Message = $"Feed notification ({request.Type}) sent."
        });
    }
}
