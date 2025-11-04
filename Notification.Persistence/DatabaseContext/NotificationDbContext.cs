using Microsoft.EntityFrameworkCore;

using Notification.Domain;

namespace Notification.Persistence.DatabaseContext;

public sealed class NotificationDbContext : DbContext
{

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
}
