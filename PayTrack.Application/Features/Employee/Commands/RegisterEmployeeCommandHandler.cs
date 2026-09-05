using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class RegisterEmployeeCommandHandler: IRequestHandler<RegisterEmployeeCommand, RegisterEmployeeResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyDetailsRepository _companyDetailsRepository;
        private readonly IEmailService _emailService;

        public RegisterEmployeeCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmployeesRepository employeesRepository,
            RoleManager<IdentityRole> roleManager,
            ICurrentUserService currentUserService,
            ICompanyDetailsRepository companyDetailsRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _employeesRepository = employeesRepository;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _companyDetailsRepository = companyDetailsRepository;
            _emailService = emailService;
        }

        public async Task<RegisterEmployeeResponseDTO> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
            byte[]? profileBytes = null;

           
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

            const string employeeRole = "Employee";

     
            var verificationToken = Guid.NewGuid().ToString();

         
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfilePicture = profileBytes,
                Role = employeeRole,

              
                IsEmailVerified = false,
                EmailConfirmed = false,
                EmailVerificationToken = verificationToken
            };

          
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(x => x.Description)));
            }

           
            if (!await _roleManager.RoleExistsAsync(employeeRole))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(employeeRole));
            }

          
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                employeeRole);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        roleResult.Errors.Select(x => x.Description)));
            }

           
            var employee = new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = GenerateEmployeeNumber(),
                DepartmentId = request.DepartmentId,
                DateHired = request.DateHired,
                Role = employeeRole,
                ApplicationUserId = user.Id,

               
                CompanyAdminId = _currentUserService.UserId,

                UseCompanyLatePolicy = true,
                AttendancePolicyId = null
            };

            await _employeesRepository.AddAsync(employee);

          
            await _emailService.SendEmployeeVerificationEmailAsync(
                user.Email!,
                user.FullName,
                companyName,
                verificationToken);

          
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