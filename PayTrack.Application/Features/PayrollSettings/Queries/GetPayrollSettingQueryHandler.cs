using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Application.Features.PayrollSettings.Queries
{
    public class GetPayrollSettingQueryHandler
        : IRequestHandler<GetPayrollSettingQuery, PayrollSettingDTO?>
    {
        private readonly ICompanyPayrollSettingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetPayrollSettingQueryHandler(
            ICompanyPayrollSettingRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PayrollSettingDTO?> Handle(
            GetPayrollSettingQuery request,
            CancellationToken cancellationToken)
        {       
            var companyAdminId = _currentUserService.UserId!;

            var setting =
                await _repository.GetByCompanyAdminIdAsync(companyAdminId);

            if (setting == null)
            {
                return null;
            }

            return new PayrollSettingDTO(
                setting.Frequency
            );
        }
    }
}