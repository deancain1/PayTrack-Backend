using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IQrCodeService _qrCodeService;
        public RegisterEmployeeCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmployeesRepository employeesRepository, RoleManager<IdentityRole> roleManager, IQrCodeService qrCodeService )
        {
            _userManager = userManager;
            _employeesRepository = employeesRepository;
            _roleManager = roleManager;
            _qrCodeService = qrCodeService;
        }
        public async Task<AuthResponseDTO> Handle(
          RegisterEmployeeCommand request,
          CancellationToken cancellationToken)
        {
            byte[]? profileBytes = null;

            if (!string.IsNullOrWhiteSpace(request.ProfilePicture))
            {
                var base64 = request.ProfilePicture.Contains(',')
                    ? request.ProfilePicture.Split(',')[1]
                    : request.ProfilePicture;

                profileBytes = Convert.FromBase64String(base64);
            }

            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfilePicture = profileBytes,
                Role = request.Role,
            };

            var result = await _userManager.CreateAsync(
                user,
                request.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                    result.Errors.Select(x => x.Description)));
            }

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(request.Role)
                );
            }

            await _userManager.AddToRoleAsync(user, request.Role);

            var employee = new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = GenerateEmployeeNumber(),
                Department = request.Department,
                DateHired = request.DateHired,
                Role = request.Role,
                ApplicationUserId = user.Id
            };

            await _employeesRepository.AddEmployeeAsync(
                employee);

            employee.QrCodeBase64 =
             _qrCodeService.GenerateQrCodeBase64(employee.Id.ToString());

            await _employeesRepository.UpdateAsync(employee);


            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Department = employee.Department,
                EmployeeNumber = employee.EmployeeNumber,
                UserId = user.Id,
                Role = "Employee",
               
            };
        }

        private string GenerateEmployeeNumber()
        {
            return $"EMP-{DateTime.Now.Ticks}";
        }
    }
}
