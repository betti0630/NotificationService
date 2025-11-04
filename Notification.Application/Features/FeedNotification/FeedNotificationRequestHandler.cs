using MediatR;

using Microsoft.EntityFrameworkCore;

using Notification.Application.Features.Registration.SendRegistrationEmail;
using Notification.Domain;
using Notification.Persistence.DatabaseContext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Features.FeedNotification;

internal class FeedNotificationRequestHandler : IRequestHandler<FeedNotificationAppRequest, FeedNotificationAppResponse>
{
    private readonly IDbContextFactory<NotificationDbContext> _dbContextFactory;

    public FeedNotificationRequestHandler(IDbContextFactory<NotificationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<FeedNotificationAppResponse> Handle(FeedNotificationAppRequest request, CancellationToken cancellationToken)
    {
        var notification = new UserNotification()
        {
            Id = Guid.NewGuid(),
            FromUserId = request.FromUserId,
            ToUserId = request.ToUserId,
            Message = request.Message,
            Date = DateTime.Now
        };
        using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.UserNotifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);
        var response = new FeedNotificationAppResponse();
        response.Success = true;
        return response;
    }
}
