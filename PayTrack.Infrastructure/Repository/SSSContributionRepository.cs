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
    public class SSSContributionRepository : ISSSContributionRepository
    {
        private readonly AppDbContext _context;
        public SSSContributionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SSSContribution?> GetContributionAsync(decimal monthlySalary)
        {
            return await _context.SSSContributions
              .FirstOrDefaultAsync(x =>
                  monthlySalary >= x.MinSalary &&
                  (
                      x.MaxSalary == null ||
                      monthlySalary <= x.MaxSalary
                  ));
        }
    }
}
