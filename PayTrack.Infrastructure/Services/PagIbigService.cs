using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;

namespace PayTrack.Infrastructure.Services
{
    public class PagIbigService : IPagIbigService
    {
        private readonly IPagIbigContributionRepository _repository;

        public PagIbigService(
            IPagIbigContributionRepository repository)
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

            // Apply Pag-IBIG maximum compensation
            if (contributionSalary > settings.MaximumCompensation)
            {
                contributionSalary = settings.MaximumCompensation;
            }

            // Determine employee contribution rate
            decimal employeeRate =
                contributionSalary <= settings.SalaryThreshold
                    ? settings.LowerEmployeeRate
                    : settings.HigherEmployeeRate;

            // Monthly employee contribution
            decimal monthlyContribution =
                contributionSalary * employeeRate;

            // Deduct per payroll frequency
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