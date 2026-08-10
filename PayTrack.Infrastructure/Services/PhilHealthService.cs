using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Services
{
    public class PhilHealthService : IPhilHealthService
    {
        private readonly IPhilHealthContributionRepository _repository;

        public PhilHealthService(
            IPhilHealthContributionRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> ComputeEmployeeContributionAsync(
            decimal monthlySalary,
            PayrollFrequency frequency)
        {
            var settings =
                await _repository.GetContributionAsync();

            if (settings == null)
                return 0m;

            // Monthly salary is already the contribution basis
            decimal contributionSalary = monthlySalary;

            // Apply salary floor
            if (contributionSalary < settings.MinSalary)
            {
                contributionSalary = settings.MinSalary;
            }

            // Apply salary ceiling
            if (contributionSalary > settings.MaxSalary)
            {
                contributionSalary = settings.MaxSalary;
            }

            // Compute total monthly contribution
            decimal monthlyContribution =
                contributionSalary * settings.ContributionRate;

            // Employee share only
            decimal employeeContribution =
                monthlyContribution * settings.EmployeeSharePercentage;

            // Deduct per payroll frequency
            return frequency switch
            {
                PayrollFrequency.Weekly =>
                    employeeContribution / 4m,

                PayrollFrequency.SemiMonthly =>
                    employeeContribution / 2m,

                PayrollFrequency.Monthly =>
                    employeeContribution,

                _ => employeeContribution
            };
        }
    }
}