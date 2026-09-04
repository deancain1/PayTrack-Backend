using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class RegisterEmployeeCommandHandler: IRequestHandler< RegisterEmployeeCommand,RegisterEmployeeResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyDetailsRepository _companyDetailsRepository;

        public RegisterEmployeeCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmployeesRepository employeesRepository,
            RoleManager<IdentityRole> roleManager,
            ICurrentUserService currentUserService,
            ICompanyDetailsRepository companyDetailsRepository)
        {
            _userManager = userManager;
            _employeesRepository = employeesRepository;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _companyDetailsRepository = companyDetailsRepository;
        }

        public async Task<RegisterEmployeeResponseDTO> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
            byte[]? profileBytes = null;

            // Convert profile picture
            if (!string.IsNullOrWhiteSpace(request.ProfilePicture))
            {
                var base64 = request.ProfilePicture.Contains(',')
                    ? request.ProfilePicture.Split(',')[1]
                    : request.ProfilePicture;

                profileBytes = Convert.FromBase64String(base64);
            }

            var companyAdminId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(companyAdminId))
            {
                throw new Exception(
                    "Unable to identify the current Company Admin.");
            }

            // Get company details of the current Company Admin
            var companyDetails =
                await _companyDetailsRepository
                    .GetCompanyDetailsByCompanyAdminAsync(companyAdminId);

            if (companyDetails == null)
            {
                throw new Exception(
                    "Company details not found.");
            }

            var companyName = companyDetails.CompanyName;

            // Employee role is controlled by the backend
            const string employeeRole = "Employee";

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfilePicture = profileBytes,
                Role = employeeRole
            };

            // Create Identity user
            var result = await _userManager.CreateAsync(
                user,
                request.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(x => x.Description)));
            }

            // Create role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(employeeRole))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(employeeRole));
            }

            // Assign Employee role
            await _userManager.AddToRoleAsync(
                user,
                employeeRole);

            // Create Employee record
            var employee = new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = GenerateEmployeeNumber(),
                DepartmentId = request.DepartmentId,
                DateHired = request.DateHired,
                Role = employeeRole,
                ApplicationUserId = user.Id,

                // Company Admin who registered this employee
                CompanyAdminId = _currentUserService.UserId,

                UseCompanyLatePolicy = true,
                AttendancePolicyId = null
            };

            await _employeesRepository.AddAsync(employee);

            // Return employee-specific response
            return new RegisterEmployeeResponseDTO(
                FullName: user.FullName,
                PhoneNumber: user.PhoneNumber ?? string.Empty,
                Email: user.Email ?? string.Empty,
                DepartmentId: employee.DepartmentId,
                EmployeeNumber: employee.EmployeeNumber,
                UserId: user.Id,
                Role: employeeRole
            );
        }

        private string GenerateEmployeeNumber()
        {
            return $"EMP-{DateTime.Now.Ticks}";
        }
    }
}