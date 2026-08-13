using PayTrack.Domain.Entities;

namespace PayTrack.Application.Interfaces.Repositories;

public interface ICompanyAdminNotificationRepository : IBaseRepository<CompanyAdminNotification>
{
    Task<List<CompanyAdminNotification>> GetByCompanyAdminIdAsync(string companyAdminId);

    Task<int> GetUnreadCountAsync(string companyAdminId);

    Task MarkAsReadAsync(Guid notificationId);

    
}