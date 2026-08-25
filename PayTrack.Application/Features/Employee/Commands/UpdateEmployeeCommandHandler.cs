using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class UpdateEmployeeCommandHandler
        : IRequestHandler<UpdateEmployeeCommand, EmployeeDTO>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateEmployeeCommandHandler(
            IEmployeesRepository employeesRepository,
            UserManager<ApplicationUser> userManager)
        {
            _employeesRepository = employeesRepository;
            _userManager = userManager;
        }

        public async Task<EmployeeDTO> Handle(
            UpdateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            // 1. FIND EMPLOYEE

            var employee =
                await _employeesRepository.GetByIdWithDetailsAsync(
                    request.Id);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee not found.");
            }


            // 2. FIND ASPNET USER

            var applicationUser =
                await _userManager.FindByIdAsync(
                    employee.ApplicationUserId.ToString());

            if (applicationUser == null)
            {
                throw new KeyNotFoundException(
                    "Application user not found.");
            }


            // 3. CHECK DUPLICATE EMAIL

            var existingUser =
                await _userManager.FindByEmailAsync(
                    request.Email.Trim());

            if (existingUser != null &&
                existingUser.Id != applicationUser.Id)
            {
                throw new InvalidOperationException(
                    "Email address is already in use.");
            }


            // 4. UPDATE ASPNET USER

            applicationUser.FullName =
                request.FullName.Trim();

            applicationUser.PhoneNumber =
                request.PhoneNumber.Trim();


            // 5. UPDATE EMAIL

            if (!string.Equals(
                    applicationUser.Email,
                    request.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                var emailResult =
                    await _userManager.SetEmailAsync(
                        applicationUser,
                        request.Email.Trim());

                if (!emailResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        emailResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Unable to update email: {errors}");
                }


                // UPDATE USERNAME

                var usernameResult =
                    await _userManager.SetUserNameAsync(
                        applicationUser,
                        request.Email.Trim());

                if (!usernameResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        usernameResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Unable to update username: {errors}");
                }
            }


            // 6. UPDATE IDENTITY USER

            var userResult =
                await _userManager.UpdateAsync(
                    applicationUser);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to update user: {errors}");
            }


            // 7. UPDATE EMPLOYEE DEPARTMENT

            employee.DepartmentId =
                request.DepartmentId;


            // 8. SAVE EMPLOYEE

            _employeesRepository.Update(employee);

            await _employeesRepository.SaveChangesAsync();


            // 9. RELOAD UPDATED EMPLOYEE

            var updatedEmployee =
                await _employeesRepository.GetByIdWithDetailsAsync(
                    employee.Id);

            if (updatedEmployee == null)
            {
                throw new KeyNotFoundException(
                    "Updated employee could not be loaded.");
            }


            // 10. RETURN DTO

            return new EmployeeDTO(
                updatedEmployee.Id,
                updatedEmployee.ApplicationUserId,
                updatedEmployee.ApplicationUser.FullName,
                updatedEmployee.ApplicationUser.Email,
                updatedEmployee.EmployeeNumber,
                updatedEmployee.DepartmentId,
                updatedEmployee.Department?.DepartmentName ?? string.Empty,
                updatedEmployee.ApplicationUser.PhoneNumber,
                updatedEmployee.ApplicationUser.ProfilePicture != null
                    ? Convert.ToBase64String(
                        updatedEmployee.ApplicationUser.ProfilePicture)
                    : null,
                updatedEmployee.QrCodeBase64
            );
        }
    }
}