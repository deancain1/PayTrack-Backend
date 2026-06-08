using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Commands
{
    public class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, Guid>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IPhotoStorageService _photoStorageService;

        public RecordAttendanceCommandHandler(
            IEmployeesRepository employeesRepository,
            IAttendanceRepository attendanceRepository,
            IPhotoStorageService photoStorageService)
        {
            _employeesRepository = employeesRepository;
            _attendanceRepository = attendanceRepository;
            _photoStorageService = photoStorageService;
        }

        public async Task<Guid> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
        {
           var employee = await _employeesRepository.GetByIdAndNumberAsync(request.Id, request.EmployeeNumber);
            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }
            var photoPath = await _photoStorageService
               .SaveAttendancePhotoAsync(request.PhotoBase64);

            var attendance = new Attendances
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ScanTime = DateTime.UtcNow,
                PhotoPath = photoPath
            };

            await _attendanceRepository.AddAsync(attendance);

            return attendance.Id;
        }
    }
}
