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
        private readonly ISalaryRepository _salaryRepository;
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly IPayrollService _payrollService;

        public RecordAttendanceCommandHandler(
            IEmployeesRepository employeesRepository,
            IAttendanceRepository attendanceRepository,
            IPhotoStorageService photoStorageService,
            ISalaryRepository salaryRepository,
            IPayrollRecordRepository payrollRecordRepository,
            IPayrollService payrollService)
        {
            _employeesRepository = employeesRepository;
            _attendanceRepository = attendanceRepository;
            _photoStorageService = photoStorageService;
            _salaryRepository = salaryRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _payrollService = payrollService;
        }

        public async Task<Guid> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
        {
            var employee =
                await _employeesRepository
                .GetByIdAndNumberAsync(
                    request.Id,
                    request.EmployeeNumber);

            if (employee == null)
                throw new Exception("Employee not found.");

            var photoPath =
                await _photoStorageService
                .SaveAttendancePhotoAsync(
                    request.PhotoBase64);

            var attendance =
                await _attendanceRepository
                .GetTodayAttendanceAsync(employee.Id);

            // First Scan = Time In
            if (attendance == null)
            {
                attendance = new Attendances
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Date = DateTime.UtcNow.Date,
                    TimeIn = DateTime.UtcNow,
                    TimeInPhoto = photoPath
                };

                await _attendanceRepository.AddAsync(attendance);

                return attendance.Id;
            }

            // Second Scan = Time Out
            if (attendance.TimeOut == null)
            {
                attendance.TimeOut = DateTime.UtcNow;
                attendance.TimeOutPhoto = photoPath;

                var duration = attendance.TimeOut.Value - attendance.TimeIn;

                attendance.HoursWorked = Math.Round(
                    (decimal)duration.TotalSeconds,
                    2);

                await _attendanceRepository.UpdateAsync(attendance);

                // PAYROLL LOGIC STARTS HERE

                var salary = await _salaryRepository
                    .GetByEmployeeIdAsync(employee.Id);

                var earned = _payrollService.Calculate(
                    salary,
                    attendance.HoursWorked);

                var payroll = new PayrollRecord
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    AttendanceId = attendance.Id,
                    AmountEarned = earned,
                    CreatedAt = DateTime.UtcNow
                };

                await _payrollRecordRepository.AddAsync(payroll);

                return attendance.Id;
            }

            throw new Exception(
                "Employee already timed in and timed out today.");
        }
    }
}
