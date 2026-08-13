using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollSettings.Commands
{
    public class SetPayrollSettingCommandHandler
        : IRequestHandler<SetPayrollSettingCommand, bool>
    {
        private readonly ICompanyPayrollSettingRepository _companyPayrollSettingRepository;
        private readonly ICurrentUserService _currentUserService;

        public SetPayrollSettingCommandHandler(
            ICompanyPayrollSettingRepository companyPayrollSettingRepository,
            ICurrentUserService currentUserService)
        {
            _companyPayrollSettingRepository = companyPayrollSettingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle( SetPayrollSettingCommand request, CancellationToken cancellationToken)
        {
            var companyAdminId =
                _currentUserService.UserId!;

            var setting =
                await _companyPayrollSettingRepository.GetByCompanyAdminIdAsync(
                    companyAdminId);

            if (setting == null)
            {
                setting = new CompanyPayrollSetting
                {
                    Id = Guid.NewGuid(),
                    CompanyAdminId = companyAdminId,
                    Frequency = request.Frequency
                };

                await _companyPayrollSettingRepository.AddAsync(setting);
            }
            else
            {
                setting.Frequency = request.Frequency;

                _companyPayrollSettingRepository.Update(setting);
                await _companyPayrollSettingRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
