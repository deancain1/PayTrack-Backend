using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Data;

namespace PayTrack.Infrastructure.Repository;

public class CompanyAdminNotificationRepository : BaseRepository<CompanyAdminNotification>, ICompanyAdminNotificationRepository
{
    public CompanyAdminNotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<CompanyAdminNotification>> GetByCompanyAdminIdAsync(string companyAdminId)
    {
        return await _context.Notifications
            .Where(n =>
            n.CompanyAdminId == companyAdminId &&
            (
                n.Type == CompanyAdminNotificationType.SalaryAdvance ||
                (
                    (n.Type == CompanyAdminNotificationType.TimeIn ||
                     n.Type == CompanyAdminNotificationType.TimeOut) &&
                    n.CreatedAt >= DateTime.UtcNow.AddHours(-24)
                )
            ))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string companyAdminId)
    {
        return await _context.Notifications
            .CountAsync(x =>
                x.CompanyAdminId == companyAdminId &&
                !x.IsRead);
    }

    //public async Task<Notification?> GetByIdAsync(Guid notificationId)
    //{
    //    return await _context.Notifications
    //        .FirstOrDefaultAsync(x => x.Id == notificationId);
    //}

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == notificationId);

        if (notification == null)
            return;

        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    //public async Task MarkAllAsReadAsync(string companyAdminId)
    //{
    //    var notifications = await _context.Notifications
    //        .Where(x =>
    //            x.CompanyAdminId == companyAdminId &&
    //            !x.IsRead)
    //        .ToListAsync();

    //    foreach (var notification in notifications)
    //    {
    //        notification.IsRead = true;
    //    }

    //    await _context.SaveChangesAsync();
    //}

    //public async Task DeleteAsync(Guid notificationId)
    //{
    //    var notification = await _context.Notifications
    //        .FirstOrDefaultAsync(x => x.Id == notificationId);

    //    if (notification == null)
    //        return;

    //    _context.Notifications.Remove(notification);

    //    await _context.SaveChangesAsync();
    //}
}