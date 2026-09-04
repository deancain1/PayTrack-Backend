using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface ICompanyDetailsRepository : IBaseRepository<CompanyDetails>
    {
       
        Task<CompanyDetails?> GetCompanyDetailsByCompanyAdminAsync(string companyAdminId);
    }
}
