using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Users.Queries
{
    public class GetUserByRoleQueryHandler
        : IRequestHandler<GetUserByRoleQuery, PagedResult<UserDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmployeesRepository _employeesRepository;

        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 20;

        public GetUserByRoleQueryHandler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmployeesRepository employeesRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _employeesRepository = employeesRepository;
        }

        public async Task<PagedResult<UserDTO>> Handle(
            GetUserByRoleQuery request,
            CancellationToken cancellationToken)
        {
            // Get all users in the specified role
            var usersInRole = await _userManager
                .GetUsersInRoleAsync(request.RoleName);

            var userIds = usersInRole
                .Select(u => u.Id)
                .ToList();

            // Query employees with ApplicationUser included
            IQueryable<Employees> query = _employeesRepository
                .GetEmployeesWithApplicationUser()
                .Where(e => userIds.Contains(e.ApplicationUserId));

            // Search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();

                query = query.Where(e =>
                    e.ApplicationUser.FullName.ToLower().Contains(term) ||
                    e.ApplicationUser.Email!.ToLower().Contains(term) ||
                    e.EmployeeNumber.ToLower().Contains(term));
            }

            // Total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination
            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize)
                : DefaultPageSize;

            int pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            // Get paginated employees
            var employees = await query
                .OrderBy(e => e.ApplicationUser.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

           
            var result = new List<UserDTO>();

            foreach (var employee in employees)
            {
                var roles = await _userManager
                    .GetRolesAsync(employee.ApplicationUser);

                var dto = new UserDTO
                {
                    UserId = employee.ApplicationUser.Id,
                    FullName = employee.ApplicationUser.FullName,
                    Email = employee.ApplicationUser.Email ?? string.Empty,

                    Role = request.RoleName,
                    Roles = roles,

                    ProfilePictureBase64 = employee.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(employee.ApplicationUser.ProfilePicture)}"
                    : null,

                    EmployeeNumber = employee.EmployeeNumber,
                    Department = employee.Department,
                    //Status = employee.Status
                };

                result.Add(dto);
            }

            return new PagedResult<UserDTO>(
                result,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}