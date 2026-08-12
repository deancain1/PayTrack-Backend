using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public class UpdateEmployeeSalaryCommandHandler : IRequestHandler<UpdateEmployeeSalaryCommand, bool>
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateEmployeeSalaryCommandHandler(ISalaryRepository salaryRepository, ICurrentUserService currentUserService)
        {
            _salaryRepository = salaryRepository;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(UpdateEmployeeSalaryCommand request, CancellationToken cancellationToken)
        {
          var companyAdminId = _currentUserService.UserId;

            var salary = await _salaryRepository.GetByIdAndCompanyAdminAsync(request.Id, companyAdminId);
            if (salary == null)
                throw new Exception("salary not found");

            salary.Rate = request.rate;
            salary.Policy = request.policy;

             _salaryRepository.Update(salary);
             await _salaryRepository.SaveChangesAsync();    
            return true;
        }
    }
}
