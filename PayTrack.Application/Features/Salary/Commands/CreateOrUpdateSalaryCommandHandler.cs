using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public class CreateOrUpdateSalaryCommandHandler: IRequestHandler<CreateOrUpdateSalaryCommand, SalaryDTO>
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IEmployeesRepository _employeeRepository;
        public CreateOrUpdateSalaryCommandHandler(ISalaryRepository salaryRepository, IEmployeesRepository employeeRepository)
        {
            _salaryRepository = salaryRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<SalaryDTO> Handle(CreateOrUpdateSalaryCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

            if (employee == null)
                throw new Exception($"Employee '{request.EmployeeId}' not found.");

            var existing = await _salaryRepository.GetByEmployeeIdAsync(employee.Id);

            EmployeeSalary salary;

            if (existing == null)
            {
                salary = new EmployeeSalary
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Rate = request.Rate,
                    Policy = request.Policy
                };

                await _salaryRepository.AddAsync(salary);
            }
            else
            {
                existing.Rate = request.Rate;
                existing.Policy = request.Policy;

                await _salaryRepository.UpdateAsync(existing);

                salary = existing;
            }

            return new SalaryDTO(
      EmployeeId: employee.Id,
      FullName: employee.ApplicationUser.FullName,
      EmployeeNumber: employee.EmployeeNumber,
      Department: employee.Department?.DepartmentName ?? "Unassigned",
      Rate: salary.Rate,
      ProfilePictureBase64: employee.ApplicationUser.ProfilePicture != null
          ? $"data:image/png;base64,{Convert.ToBase64String(employee.ApplicationUser.ProfilePicture)}"
          : null,
      Policy: salary.Policy
  );
        }
    }
}
