using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class PayrollService : IPayrollService
    {
        public decimal Calculate(EmployeeSalary salary, Attendances attendance, WorkSchedules schedule)
        {
            decimal earned = 0;

            var scheduledEnd = attendance.Date + schedule.EndTime;

            bool completedShift =
                attendance.TimeOut.HasValue &&
                attendance.TimeOut.Value >= scheduledEnd;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:

                    earned = salary.Rate * attendance.HoursWorked;

                    break;

                case PayrollPolicy.ProRated:

                    earned =
                        (salary.Rate / schedule.RequiredHours)
                        * attendance.HoursWorked;

                    break;

                case PayrollPolicy.FullDay:

                    if (completedShift)
                        earned = salary.Rate;
                    else
                        earned = 0;

                    break;

                case PayrollPolicy.HalfDay:

                    if (completedShift)
                        earned = salary.Rate;
                    else if (attendance.HoursWorked >= schedule.RequiredHours / 2m)
                        earned = salary.Rate / 2m;
                    else
                        earned = 0;

                    break;

                case PayrollPolicy.Monthly:

                    earned = salary.Rate;

                    break;
            }

            return Math.Round(earned, 2);
        }
    }
}
