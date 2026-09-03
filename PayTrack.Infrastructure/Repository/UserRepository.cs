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
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }


        public async Task DeactivateUserAsync(ApplicationUser user)
        {
            user.IsActive = false;

            await _context.SaveChangesAsync();
        }
        public async Task ReactivateUserAsync(ApplicationUser user)
        {
            user.IsActive = true;

            await _context.SaveChangesAsync();
        }

    }
}
