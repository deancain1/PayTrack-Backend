using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Commands
{
    public class CreateSalaryCommandHandler: IRequestHandler<CreateSalaryCommand, SalaryDTO>
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        public CreateSalaryCommandHandler(ISalaryRepository salaryRepository, IEmployeesRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _salaryRepository = salaryRepository;
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<SalaryDTO> Handle(CreateSalaryCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdWithDetailsAsync(request.EmployeeId);

            if (employee == null)
                throw new Exception($"Employee '{request.EmployeeId}' not found.");

            var existing = await _salaryRepository.GetByEmployeeIdAsync(employee.Id);

            if (existing != null)
                throw new Exception("This employee already has a salary assigned.");

            var salary = new EmployeeSalary
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                Rate = request.Rate,
                MonthlyRate = request.MonthlyRate,
                Policy = request.Policy,
                CompanyAdminId = _currentUserService.UserId
            };

            await _salaryRepository.AddAsync(salary);

            return new SalaryDTO(
                Id: salary.Id,
                EmployeeId: employee.Id,
                FullName: employee.ApplicationUser.FullName,
                EmployeeNumber: employee.EmployeeNumber,
                Department: employee.Department?.DepartmentName ?? "Unassigned",
                MonthlyRate: salary.MonthlyRate,
                Rate: salary.Rate,
                ProfilePictureBase64: employee.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(employee.ApplicationUser.ProfilePicture)}"
                    : null,
                Policy: salary.Policy
            );
        }
    }
}
