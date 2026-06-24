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
        public decimal Calculate(EmployeeSalary salary, decimal hoursWorked, int requiredHours)
        {
            decimal earned = 0;

            switch (salary.Policy)
            {
                case PayrollPolicy.Hourly:

                    earned =
                        salary.Rate * hoursWorked;

                    break;

                case PayrollPolicy.ProRated:

                    earned =
                        (salary.Rate / requiredHours)
                        * hoursWorked;

                    break;

                case PayrollPolicy.FullDay:

                    earned =
                        hoursWorked >= requiredHours
                        ? salary.Rate
                        : 0;

                    break;

                case PayrollPolicy.HalfDay:

                    if (hoursWorked >= requiredHours)
                    {
                        earned = salary.Rate;
                    }
                    else if (
                        hoursWorked >=
                        requiredHours / 2m)
                    {
                        earned =
                            salary.Rate / 2m;
                    }
                    else
                    {
                        earned = 0;
                    }

                    break;

                case PayrollPolicy.Monthly:

                    earned = salary.Rate;

                    break;
            }

            return Math.Round(earned, 2);
        }
    }
}
