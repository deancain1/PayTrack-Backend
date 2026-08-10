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
    public class PhilHealthContributionRepository : IPhilHealthContributionRepository
    {
        private readonly AppDbContext _context;
        public PhilHealthContributionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PhilHealthContribution?>GetContributionAsync()
        {
            return await _context
                .PhilHealthContributions
                .FirstOrDefaultAsync();
        }
    }
}
