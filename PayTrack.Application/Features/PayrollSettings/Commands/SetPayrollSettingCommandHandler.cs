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
        private readonly ICompanyPayrollSettingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public SetPayrollSettingCommandHandler(
            ICompanyPayrollSettingRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle( SetPayrollSettingCommand request, CancellationToken cancellationToken)
        {
            var companyAdminId =
                _currentUserService.UserId!;

            var setting =
                await _repository.GetByCompanyAdminIdAsync(
                    companyAdminId);

            if (setting == null)
            {
                setting = new CompanyPayrollSetting
                {
                    Id = Guid.NewGuid(),
                    CompanyAdminId = companyAdminId,
                    Frequency = request.Frequency
                };

                await _repository.AddAsync(setting);
            }
            else
            {
                setting.Frequency = request.Frequency;

                await _repository.UpdateAsync(setting);
            }

            return true;
        }
    }
}
