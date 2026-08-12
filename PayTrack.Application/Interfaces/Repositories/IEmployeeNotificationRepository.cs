using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IEmployeeNotificationRepository
    {
        Task AddAsync(EmployeeNotification notification);
        Task<List<EmployeeNotification>> GetByUserIdAsync(string applicationUserId);
        Task<EmployeeNotification?> GetByIdAsync(Guid id,string applicationUserId);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(string applicationUserId);
        Task<int> GetUnreadCountAsync(string applicationUserId);
    }
}
