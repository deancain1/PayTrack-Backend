using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Company.Queries
{
    internal class GetCompanyDetailsQueryHandler : IRequestHandler<GetCompanyDetailsQuery, CompanyDetailsDTO>
    {
        private readonly ICompanyDetailsRepository _companyDetailsRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCompanyDetailsQueryHandler(ICompanyDetailsRepository companyDetailsRepository, ICurrentUserService currentUserService)
        {
            _companyDetailsRepository = companyDetailsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<CompanyDetailsDTO> Handle(GetCompanyDetailsQuery request, CancellationToken cancellationToken)
        {
            var companyDetails = await _companyDetailsRepository.GetCompanyDetailsByCompanyAdminAsync(
                _currentUserService.UserId);

            if (companyDetails == null)
                throw new Exception("Company details not found.");

            return new CompanyDetailsDTO(
                companyDetails.Id,
                companyDetails.CompanyName,
                companyDetails.CompanyEmail,
                companyDetails.CompanyContactNumber,
                companyDetails.CompanyAddress,
                companyDetails.BusinessType,
                companyDetails.CompanyLogo != null
                    ? Convert.ToBase64String(companyDetails.CompanyLogo)
                    : null
                    );
        }
    }
}
