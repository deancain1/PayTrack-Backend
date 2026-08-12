using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    public class EmployeeNotificationRepository : IEmployeeNotificationRepository
    {
        private readonly AppDbContext _context;

        public EmployeeNotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
        EmployeeNotification notification)
        {
            await _context.EmployeeNotifications.AddAsync(
                notification);

            await _context.SaveChangesAsync();
        }

        public async Task<List<EmployeeNotification>> GetByUserIdAsync(
            string applicationUserId)
        {
            return await _context.EmployeeNotifications
                .AsNoTracking()
                .Where(x =>
                    x.ApplicationUserId == applicationUserId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeNotification?> GetByIdAsync(
            Guid id,
            string applicationUserId)
        {
            return await _context.EmployeeNotifications
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.ApplicationUserId == applicationUserId);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification =
                await _context.EmployeeNotifications
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
                return;

            notification.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(
            string applicationUserId)
        {
            var notifications =
                await _context.EmployeeNotifications
                    .Where(x =>
                        x.ApplicationUserId == applicationUserId &&
                        !x.IsRead)
                    .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(
            string applicationUserId)
        {
            return await _context.EmployeeNotifications
                .CountAsync(x =>
                    x.ApplicationUserId == applicationUserId &&
                    !x.IsRead);
        }
    }

}

