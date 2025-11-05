using MediatR;

using Microsoft.EntityFrameworkCore;

using Notification.Persistence.DatabaseContext;

namespace Notification.Application.Features.FeedNotification;

internal class SetNotificationReadRequestHandler : IRequestHandler<SetNotificationReadRequest>
{
    private readonly IDbContextFactory<NotificationDbContext> _dbContextFactory;

    public SetNotificationReadRequestHandler(IDbContextFactory<NotificationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    async Task IRequestHandler<SetNotificationReadRequest>.Handle(SetNotificationReadRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var notifications = await dbContext.UserNotifications
            .Where(n => n.ToUserId == request.UserId && !n.IsRead)
            .ToListAsync(cancellationToken);
        if (notifications.Count != 0)
        {
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
