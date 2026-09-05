using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;

namespace PayTrack.Infrastructure.Services
{
    public class TimeOutReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TimeOutReminderBackgroundService> _logger;

        // Employee gets the reminder 15 minutes after
        // their expected end time.
        private static readonly TimeSpan ReminderGracePeriod =
            TimeSpan.FromMinutes(15);

        public TimeOutReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<TimeOutReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Time Out Reminder Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    await ProcessTimeOutRemindersAsync(
                        scope.ServiceProvider,
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing Time Out reminders.");
                }

                // Check every 5 minutes
                await Task.Delay(
                    TimeSpan.FromMinutes(5),
                    stoppingToken);
            }

            _logger.LogInformation(
                "Time Out Reminder Background Service stopped.");
        }

        private async Task ProcessTimeOutRemindersAsync(
            IServiceProvider services,
            CancellationToken cancellationToken)
        {
            var employeeRepository =
                services.GetRequiredService<IEmployeesRepository>();

            var attendanceRepository =
                services.GetRequiredService<IAttendanceRepository>();

            var overtimeRepository =
                services.GetRequiredService<IOvertimeRepository>();

            var emailService =
                services.GetRequiredService<IEmailService>();

            // Get all employees
            var employees =
                await employeeRepository.GetAllAsync();

            // Use Philippine/local server time
            var now = DateTime.Now;

            var today = now.Date;

            var currentTime = now.TimeOfDay;

            foreach (var employee in employees)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await ProcessEmployeeAsync(
                        employee,
                        today,
                        currentTime,
                        attendanceRepository,
                        overtimeRepository,
                        emailService);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing Time Out reminder for employee {EmployeeId}.",
                        employee.Id);
                }
            }
        }

        private async Task ProcessEmployeeAsync(
            Employees employee,
            DateTime today,
            TimeSpan currentTime,
            IAttendanceRepository attendanceRepository,
            IOvertimeRepository overtimeRepository,
            IEmailService emailService)
        {
            _logger.LogInformation(
                "Checking Time Out reminder for employee {EmployeeId} at {CurrentTime}",
                employee.Id,
                currentTime);

   

            var attendance =
                await attendanceRepository.GetTodayAttendanceAsync(
                    employee.Id);

            if (attendance == null)
            {
                _logger.LogInformation(
                    "No attendance found today for employee {EmployeeId}",
                    employee.Id);

                return;
            }

            _logger.LogInformation(
                "Attendance found. TimeIn={TimeIn}, TimeOut={TimeOut}, ReminderSent={ReminderSent}",
                attendance.TimeIn,
                attendance.TimeOut,
                attendance.TimeOutReminderSentAt);



            if (attendance.TimeIn == null)
            {
                _logger.LogInformation(
                    "Employee {EmployeeId} has not timed in.",
                    employee.Id);

                return;
            }



            if (attendance.TimeOut != null)
            {
                _logger.LogInformation(
                    "Employee {EmployeeId} already timed out at {TimeOut}.",
                    employee.Id,
                    attendance.TimeOut);

                return;
            }



            if (attendance.TimeOutReminderSentAt != null)
            {
                _logger.LogInformation(
                    "Reminder already sent to employee {EmployeeId} at {SentAt}.",
                    employee.Id,
                    attendance.TimeOutReminderSentAt);

                return;
            }


            var schedule = employee.WorkSchedule;

            if (schedule == null)
            {
                _logger.LogWarning(
                    "No work schedule found for employee {EmployeeId}.",
                    employee.Id);

                return;
            }

            _logger.LogInformation(
                "Employee {EmployeeId} schedule ends at {EndTime}.",
                employee.Id,
                schedule.EndTime);


            var overtime =
                await overtimeRepository.GetAcceptedOvertimeAsync(
                    employee.Id,
                    today);

            if (overtime != null)
            {
                _logger.LogInformation(
                    "Employee {EmployeeId} has overtime ending at {OvertimeEndTime}. Status={Status}",
                    employee.Id,
                    overtime.EndTime,
                    overtime.Status);
            }
            else
            {
                _logger.LogInformation(
                    "Employee {EmployeeId} has no active overtime.",
                    employee.Id);
            }


 
            var effectiveEndTime =
                overtime?.EndTime ?? schedule.EndTime;

            if (!effectiveEndTime.HasValue)
            {
                _logger.LogWarning(
                    "No effective end time found for employee {EmployeeId}.",
                    employee.Id);

                return;
            }

            var endTime = effectiveEndTime.Value;


            var reminderTime =
                endTime.Add(ReminderGracePeriod);


            _logger.LogInformation(
                "Employee {EmployeeId}: CurrentTime={CurrentTime}, EffectiveEnd={EffectiveEnd}, ReminderTime={ReminderTime}",
                employee.Id,
                currentTime,
                endTime,
                reminderTime);



            if (currentTime < reminderTime)
            {
                _logger.LogInformation(
                    "Too early to send reminder to employee {EmployeeId}.",
                    employee.Id);

                return;
            }


            var user = employee.ApplicationUser;

            if (user == null)
            {
                _logger.LogWarning(
                    "No ApplicationUser loaded for employee {EmployeeId}.",
                    employee.Id);

                return;
            }


        
            // =====================================================

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.LogWarning(
                    "Employee {EmployeeId} has no email address.",
                    employee.Id);

                return;
            }


       

            _logger.LogInformation(
                "Sending Time Out reminder to {Email} for employee {EmployeeId}.",
                user.Email,
                employee.Id);

            await emailService.SendTimeOutReminderEmailAsync(
                user.Email,
                user.FullName,
                schedule.EndTime,
                overtime?.EndTime);


    

            attendance.TimeOutReminderSentAt =
                DateTime.UtcNow;

            await attendanceRepository.UpdateAsync(
                attendance);

            _logger.LogInformation(
                "Time Out reminder successfully sent to {Email}.",
                user.Email);
        }
    }
}