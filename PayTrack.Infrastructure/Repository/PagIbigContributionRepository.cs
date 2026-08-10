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
    public class PagIbigContributionRepository : IPagIbigContributionRepository
    {
        private readonly AppDbContext _context;

        public PagIbigContributionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagIbigContribution?> GetContributionAsync()
        {
            return await _context
                .PagIbigContributions
                .FirstOrDefaultAsync();
        }
    }
}
