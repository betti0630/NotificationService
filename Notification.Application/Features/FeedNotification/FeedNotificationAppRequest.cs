using MediatR;

using Notification.Application.Features.Registration.SendRegistrationEmail;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Features.FeedNotification;

public class FeedNotificationAppRequest : IRequest<FeedNotificationAppResponse>
{
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Message { get; set; } = default!;
}

public class FeedNotificationAppResponse
{
    public bool Success { get; set; }
}