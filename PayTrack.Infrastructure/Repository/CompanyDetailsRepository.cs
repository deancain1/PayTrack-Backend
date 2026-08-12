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
    public class CompanyDetailsRepository : BaseRepository<CompanyDetails>, ICompanyDetailsRepository
    {
        public CompanyDetailsRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(CompanyDetails companyDetails)
        {
            await _context.CompanyDetails.AddAsync(companyDetails);
            await _context.SaveChangesAsync();
        }
        public async Task<CompanyDetails?> GetCompanyDetailsByCompanyAdminIdAsync(string companyAdminId)
        {
            return await _context.CompanyDetails
                .Include(c => c.ApplicatonUser)
                .FirstOrDefaultAsync(x => x.CompanyAdminId == companyAdminId);
        }
    }
}
