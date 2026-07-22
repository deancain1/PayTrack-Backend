using PayTrack.Domain.Entities;

namespace PayTrack.Application.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<List<Notification>> GetByCompanyAdminIdAsync(string companyAdminId);

    Task<int> GetUnreadCountAsync(string companyAdminId);

    //Task<Notification?> GetByIdAsync(Guid notificationId);

    Task MarkAsReadAsync(Guid notificationId);

    //Task MarkAllAsReadAsync(string companyAdminId);

    //Task DeleteAsync(Guid notificationId);
}