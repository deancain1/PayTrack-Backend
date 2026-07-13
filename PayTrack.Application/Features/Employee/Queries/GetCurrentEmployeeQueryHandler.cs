using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;

namespace PayTrack.Application.Features.Employee.Queries
{
    public class GetCurrentEmployeeQueryHandler
        : IRequestHandler<GetCurrentEmployeeQuery, EmployeeDTO>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentEmployeeQueryHandler(IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task<EmployeeDTO> Handle(GetCurrentEmployeeQuery request, CancellationToken cancellationToken)
        {
            var e = await _employeesRepository.GetCurrentEmployeeAsync(
                _currentUserService.UserId);

            if (e == null)
                throw new Exception("Employee not found.");

            return new EmployeeDTO(
                e.Id,
                e.ApplicationUserId.ToString(),
                e.ApplicationUser.FullName,
                e.ApplicationUser?.Email,
                e.EmployeeNumber,
                e.Department?.DepartmentName ?? string.Empty,
                e.ApplicationUser?.PhoneNumber,
                e.ApplicationUser?.ProfilePicture != null
                    ? Convert.ToBase64String(e.ApplicationUser.ProfilePicture)
                    : null,
                e.QrCodeBase64
            );
        }
    }
}