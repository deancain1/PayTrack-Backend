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
        public RegisterEmployeeCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmployeesRepository employeesRepository, RoleManager<IdentityRole> roleManager )
        {
            _userManager = userManager;
            _employeesRepository = employeesRepository;
            _roleManager = roleManager;
        }
        public async Task<AuthResponseDTO> Handle(
          RegisterEmployeeCommand request,
          CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
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


            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
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
