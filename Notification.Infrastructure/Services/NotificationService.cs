using Grpc.Common.Notification;
using Grpc.Core;

using MediatR;

using Microsoft.Extensions.Logging;

using Notification.Application.Contracts;
using Notification.Application.Features.Registration.SendRegistrationEmail;

using System.Collections.Concurrent;

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
    private static readonly ConcurrentDictionary<string, IServerStreamWriter<FeedNotificationMessage>> _subscribers = new();

    public override async Task<Empty> SendFeedNotification(FeedNotificationMessage request, ServerCallContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (_subscribers.TryGetValue(request.ToUserId, out var stream))
        {
            await stream.WriteAsync(request);
        }

        return new Empty();
    }

    public override async Task FeedSubscribe(FeedSubscribeRequest request, IServerStreamWriter<FeedNotificationMessage> responseStream, ServerCallContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        _subscribers[request.UserId] = responseStream;

        try
        {
            // Addig él, amíg a kliens le nem csatlakozik
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
        finally
        {
            _subscribers.TryRemove(request.UserId, out _);
        }
    }
}
