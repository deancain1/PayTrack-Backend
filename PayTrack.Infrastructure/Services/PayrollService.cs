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
        public decimal Calculate(
            EmployeeSalary salary,
            decimal hoursWorked)
        {
            decimal earned = 0;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:
                    earned = salary.Rate * hoursWorked;
                    break;

                case PayrollPolicy.ProRated:
                    earned = (salary.Rate / salary.RequiredHours)
                             * hoursWorked;
                    break;

                case PayrollPolicy.FullDay:
                    earned = salary.Rate;
                    break;

                case PayrollPolicy.HalfDay:

                    if (hoursWorked >= salary.RequiredHours)
                    {
                        earned = salary.Rate;
                    }
                    else if (hoursWorked >= salary.RequiredHours / 2m)
                    {
                        earned = salary.Rate / 2m;
                    }
                    else
                    {
                        earned = 0;
                    }

                    break;

                case PayrollPolicy.Monthly:
                    earned = salary.Rate;
                    break;

                default:
                    throw new InvalidOperationException(
                        "Invalid payroll policy.");
            }

            return Math.Round(earned, 2);
        }
    }
}
