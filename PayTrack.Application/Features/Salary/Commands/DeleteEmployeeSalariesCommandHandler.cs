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
    public class DeleteEmployeeSalariesCommandHandler : IRequestHandler<DeleteEmployeeSalariesCommand, Unit>
    {
        private readonly ISalaryRepository _salaryRepository;   
        private readonly ICurrentUserService _currentUserService;

        public DeleteEmployeeSalariesCommandHandler(ISalaryRepository salaryRepository, ICurrentUserService currentUserService)
        {
            _salaryRepository = salaryRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Unit> Handle(DeleteEmployeeSalariesCommand request, CancellationToken cancellationToken)
        {
          var salaries = await _salaryRepository.GetByIdsAsync(request.EmployeeSalaryIds);

            if (!salaries.Any())
                throw new Exception("No salary records found.");

            var currentUserId = _currentUserService.UserId;

            if (salaries.Any(x => x.CompanyAdminId != currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to delete one or more salary records.");
            }
            await _salaryRepository.DeleteRangeAsync(salaries);
            return Unit.Value;
        }
    }
}
