using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public class AssignWorkScheduleToEmployeeCommandHandler : IRequestHandler<AssignWorkScheduleToEmployeeCommand>
    {
        private readonly IEmployeesRepository _employeesRepository;
        public AssignWorkScheduleToEmployeeCommandHandler(IEmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }
        public async Task Handle(AssignWorkScheduleToEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository
                 .GetByIdAsync(request.EmployeeId);

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            employee.WorkScheduleId = request.WorkScheduleId;
            employee.ScheduleEffectiveDate = request.ScheduleEffectiveDate;

            await _employeesRepository.UpdateAsync(employee);
        }
    }
}
