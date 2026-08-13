using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public class AssignAttendancePolicyCommandHandler : IRequestHandler<AssignAttendancePolicyCommand>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        public AssignAttendancePolicyCommandHandler(IEmployeesRepository employeesRepository, IAttendancePolicyRepository attendancePolicyRepository)
        {
            _employeesRepository = employeesRepository;
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task Handle(AssignAttendancePolicyCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository.GetByIdAsync(request.EmployeeId);

            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            var policy = await _attendancePolicyRepository.GetByIdAsync(request.AttendancePolicyId);

            if (policy == null)
            {
                throw new Exception("Attendance policy not found");
            }

            if (policy.IsDefault)
            {
                // Employee should inherit the company policy
                employee.AttendancePolicyId = null;
                employee.UseCompanyLatePolicy = true;
            }
            else
            {
                // Employee has an override
                employee.AttendancePolicyId = policy.Id;
                employee.UseCompanyLatePolicy = false;
            }

             _employeesRepository.Update(employee);
            await _employeesRepository.SaveChangesAsync();
        }
    }
}
