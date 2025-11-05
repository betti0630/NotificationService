using MediatR;

using Microsoft.AspNetCore.Mvc;

using Notification.Application.Features.FeedNotification;
using Notification.Persistence.DatabaseContext;

namespace Notification.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<FeedNotificationDto>>> GetUserNotifications(int userId)
    {
        var request = new GetNotificatonsRequest { UserId = userId };
        var response = await _mediator.Send(request);

        return Ok(response.Notifications);
    }

    [HttpPatch("read/{userId}")]
    public async Task<ActionResult<string>> SetNotificationRead(int userId)
    {
        var request = new SetNotificationReadRequest {
            UserId = userId
        };
        await _mediator.Send(request);
        return Ok(userId.ToString());
    }
}
