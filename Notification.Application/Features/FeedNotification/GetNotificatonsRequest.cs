using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Features.FeedNotification;

public class GetNotificatonsRequest : IRequest<GetNotificationResponse>
{
    public int UserId { get; set; }
}

public class GetNotificationResponse
{
    public List<FeedNotificationDto>? Notifications { get; set; }
}

public class FeedNotificationDto
{
    public Guid Id { get; set; }
    public int ToUserId { get; set;}
    public int FromUserId { get; set;}
    public string Message { get; set; } = default!;
    public DateTime Date { get; set;}
    public bool IsRead { get; set;}
}