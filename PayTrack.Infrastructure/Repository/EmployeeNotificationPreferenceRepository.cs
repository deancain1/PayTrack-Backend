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
    public class EmployeeNotificationPreferenceRepository : BaseRepository<EmployeeNotificationPreference>, IEmployeeNotificationPreferenceRepository
    {
    public EmployeeNotificationPreferenceRepository(AppDbContext context) : base(context) { }


        public async Task<EmployeeNotificationPreference?>
         GetByApplicationUserIdAsync(
             string applicationUserId)
        {
            return await _context
                .EmployeeNotificationPreferences
                .FirstOrDefaultAsync(
                    x => x.ApplicationUserId == applicationUserId);
        }

        public async Task<EmployeeNotificationPreference>
            GetOrCreateAsync(
                string applicationUserId)
        {
            var preference =
                await GetByApplicationUserIdAsync(
                    applicationUserId);

            if (preference != null)
            {
                return preference;
            }

            preference = new EmployeeNotificationPreference
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = applicationUserId,

                SalaryAdvanceApproved = true,
                SalaryAdvanceRejected = true,
                PayrollGenerated = true,
                PayrollReleased = true
            };

            await _context
                .EmployeeNotificationPreferences
                .AddAsync(preference);

            await _context.SaveChangesAsync();

            return preference;
        }

   
    }
}

