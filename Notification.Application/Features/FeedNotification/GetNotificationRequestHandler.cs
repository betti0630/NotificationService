using MediatR;

using Microsoft.EntityFrameworkCore;

using Notification.Persistence.DatabaseContext;

namespace Notification.Application.Features.FeedNotification;

internal class GetNotificationRequestHandler : IRequestHandler<GetNotificatonsRequest, GetNotificationResponse>
{
    private readonly IDbContextFactory<NotificationDbContext> _dbContextFactory;

    public GetNotificationRequestHandler(IDbContextFactory<NotificationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<GetNotificationResponse> Handle(GetNotificatonsRequest request, CancellationToken cancellationToken)
    {
        var response = new GetNotificationResponse();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        response.Notifications = await dbContext.UserNotifications
            .Where(n => n.ToUserId == request.UserId)
            .Select(n => new FeedNotificationDto
            {
                Id = n.Id,
                ToUserId = n.ToUserId,
                FromUserId = n.FromUserId,
                Date = n.Date,
                Message = n.Message,
                IsRead = n.IsRead
            })
            .ToListAsync(cancellationToken);
        return response;
    }
}
