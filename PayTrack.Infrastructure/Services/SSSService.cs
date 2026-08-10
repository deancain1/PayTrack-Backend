using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class SSSService : ISSSService
    {
        private readonly ISSSContributionRepository _repository;


        public SSSService(
            ISSSContributionRepository repository)
        {
            _repository = repository;
        }


        public async Task<decimal> ComputeEmployeeContributionAsync(
            decimal monthlySalary,
            PayrollFrequency frequency)
        {
            var contribution =
                await _repository
                .GetContributionAsync(monthlySalary);


            if (contribution == null)
                return 0m;


            // Employee share from SSS table
            var monthlyContribution =
                contribution.EmployeeTotalContribution;


            // Convert monthly SSS contribution
            // to payroll deduction
            return frequency switch
            {
                PayrollFrequency.Weekly =>
                    monthlyContribution / 4m,

                PayrollFrequency.SemiMonthly =>
                    monthlyContribution / 2m,

                PayrollFrequency.Monthly =>
                    monthlyContribution,

                _ => monthlyContribution
            };
        }
    }
}
