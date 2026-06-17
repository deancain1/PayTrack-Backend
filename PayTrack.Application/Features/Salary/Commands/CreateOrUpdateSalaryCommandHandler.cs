using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public class CreateOrUpdateSalaryCommandHandler: IRequestHandler<CreateOrUpdateSalaryCommand, Guid>
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IEmployeesRepository _employeeRepository;
        public CreateOrUpdateSalaryCommandHandler(ISalaryRepository salaryRepository, IEmployeesRepository employeeRepository)
        {
            _salaryRepository = salaryRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Guid> Handle(CreateOrUpdateSalaryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.createSalaryDTO;

            // Find employee by EmployeeNumber
            var employee = await _employeeRepository
                .GetByIdAsync(dto.EmployeeId);

            if (employee == null)
                throw new Exception($"Employee '{dto.EmployeeId}' not found.");

            // Check if salary already exists for this employee
            var existing = await _salaryRepository
                .GetByEmployeeIdAsync(employee.Id);

            if (existing == null)
            {
                var newSalary = new EmployeeSalary
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Rate = dto.Rate,
                    RequiredHours = dto.RequiredHours,
                    Policy = dto.Policy
                };

                await _salaryRepository.AddAsync(newSalary);

                return newSalary.Id;
            }

            // Update existing salary
            existing.Rate = dto.Rate;
            existing.RequiredHours = dto.RequiredHours;
            existing.Policy = dto.Policy;

            await _salaryRepository.UpdateAsync(existing);

            return existing.Id;
        }
    }
}
