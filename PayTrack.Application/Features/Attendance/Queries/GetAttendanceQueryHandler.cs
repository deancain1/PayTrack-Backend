using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetAttendanceQueryHandler: IRequestHandler<GetAttendanceQuery, PagedResult<AttendanceDTO>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPhotoStorageService _photoStorageService;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetAttendanceQueryHandler(
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUserService,
            IPhotoStorageService photoStorageService)
        {
            _attendanceRepository =
                attendanceRepository;

            _currentUserService =
                currentUserService;

            _photoStorageService =
                photoStorageService;
        }

        public async Task<PagedResult<AttendanceDTO>> Handle(GetAttendanceQuery request, CancellationToken cancellationToken)
        {
            var query =
                _attendanceRepository
                    .GetAttendancesByCompanyAdminQuery(
                        _currentUserService.UserId
                    );

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term =
                    request.Search.Trim();

                query = query.Where(a =>
                    EF.Functions.Like(
                        a.Employee.ApplicationUser.FullName,
                        $"%{term}%"
                    ) ||
                    EF.Functions.Like(
                        a.Employee.ApplicationUser.Email!,
                        $"%{term}%"
                    ) ||
                    EF.Functions.Like(
                        a.Employee.EmployeeNumber,
                        $"%{term}%"
                    )
                );
            }

            // Total Count
            var totalCount =
                await query.CountAsync(
                    cancellationToken
                );

            // Page Size
            int pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize
                    )
                    : DefaultPageSize;

            // Page Number
            int pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;

            // Pagination
            var attendances =
                await query
                    .Skip(
                        (pageNumber - 1) * pageSize
                    )
                    .Take(pageSize)
                    .ToListAsync(
                        cancellationToken
                    );

            var items =
                new List<AttendanceDTO>();

            foreach (var attendance in attendances)
            {
                string? timeInPhoto = null;
                string? timeOutPhoto = null;

                // Time In Photo
                if (!string.IsNullOrWhiteSpace(
                    attendance.TimeInPhoto))
                {
                    var bytes =
                        await _photoStorageService
                            .GetAttendancePhotoAsync(
                                attendance.TimeInPhoto
                            );

                    if (bytes != null)
                    {
                        timeInPhoto =
                            $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    }
                }

                // Time Out Photo
                if (!string.IsNullOrWhiteSpace(
                    attendance.TimeOutPhoto))
                {
                    var bytes =
                        await _photoStorageService
                            .GetAttendancePhotoAsync(
                                attendance.TimeOutPhoto
                            );

                    if (bytes != null)
                    {
                        timeOutPhoto =
                            $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    }
                }

                // Profile Picture
                string? profilePicture = null;

                if (
                    attendance.Employee.ApplicationUser
                        .ProfilePicture != null
                )
                {
                    profilePicture =
                        $"data:image/png;base64,{Convert.ToBase64String(
                            attendance.Employee.ApplicationUser.ProfilePicture
                        )}";
                }

                items.Add(
                    new AttendanceDTO(
                        attendance.Id,
                        attendance.Date,
                        profilePicture,
                        attendance.Employee
                            .ApplicationUser
                            .FullName,
                        attendance.Employee.Department != null
                            ? attendance.Employee.Department.DepartmentName
                            : null,
                        attendance.Employee.EmployeeNumber,
                        attendance.TimeIn,
                        attendance.TimeOut,
                        attendance.Status,
                        timeInPhoto,
                        timeOutPhoto
                    )
                );
            }

            return new PagedResult<AttendanceDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}