using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Users.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeesRepository _employeesRepository;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, IEmployeesRepository employeesRepository)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
        }
        public async Task<CurrentUserDTO> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user =  await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var employee = await _employeesRepository.GetCurrentEmployeeByApplicationUserIdAsync(user.Id);

            var department = employee?.Department?.DepartmentName;


            return new CurrentUserDTO(
            user.Id.ToString(),
            user.FullName,
            user.Email!,
            roles.FirstOrDefault() ?? "",
            department,
            user.ProfilePicture != null
             ? $"data:image/png;base64,{Convert.ToBase64String(user.ProfilePicture)}"
             : null
            ); 

        }
    }
}
