using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Company.Command
{
    public class UpdateCompanyDetailsCommandHandler : IRequestHandler<UpdateCompanyDetailsCommand, CompanyDetailsDTO>
    {
        private readonly ICompanyDetailsRepository _companyDetailsRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateCompanyDetailsCommandHandler(
            ICompanyDetailsRepository companyDetailsRepository,
            ICurrentUserService currentUserService)
        {
            _companyDetailsRepository = companyDetailsRepository;
            _currentUserService = currentUserService;
        }
        public async Task<CompanyDetailsDTO> Handle(UpdateCompanyDetailsCommand request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(companyAdminId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var companyDetails =
                await _companyDetailsRepository
                    .GetCompanyDetailsByCompanyAdminAsync(
                        companyAdminId);

            if (companyDetails == null)
            {
                throw new Exception(
                    "Company details not found.");
            }

            companyDetails.CompanyName =
                request.CompanyName;

            companyDetails.CompanyEmail =
                request.CompanyEmail;

            companyDetails.CompanyContactNumber =
                request.CompanyContactNumber;

            companyDetails.CompanyAddress =
                request.CompanyAddress;

            companyDetails.BusinessType =
                request.BusinessType;

            if (!string.IsNullOrWhiteSpace(request.CompanyLogo))
            {
                var base64 = request.CompanyLogo.Contains(',')
                    ? request.CompanyLogo.Split(',')[1]
                    : request.CompanyLogo;

                companyDetails.CompanyLogo =
                    Convert.FromBase64String(base64);
            }

             _companyDetailsRepository.Update(
                companyDetails);
            await _companyDetailsRepository.SaveChangesAsync();
            return new CompanyDetailsDTO(
                companyDetails.Id,
                companyDetails.CompanyName,
                companyDetails.CompanyEmail,
                companyDetails.CompanyContactNumber,
                companyDetails.CompanyAddress,
                companyDetails.BusinessType,
                companyDetails.CompanyLogo != null
                    ? Convert.ToBase64String(
                        companyDetails.CompanyLogo)
                    : null
            );
        }
    }
}

