using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Queries
{
    public class GetSalarySummaryQueryHandler: IRequestHandler<GetSalarySummaryQuery, SalarySummaryDTO>
    {
        private readonly ISalaryRepository _salaryRepository;

        public GetSalarySummaryQueryHandler(
            ISalaryRepository salaryRepository)
        {
            _salaryRepository = salaryRepository;
        }

        public async Task<SalarySummaryDTO> Handle(GetSalarySummaryQuery request,CancellationToken cancellationToken)
        {
            var salaries = await _salaryRepository.GetAllAsync();

            var fullDay = salaries
                .Where(x => x.Policy == PayrollPolicy.FullDay)
                .ToList();

            var halfDay = salaries
                .Where(x => x.Policy == PayrollPolicy.HalfDay)
                .ToList();

            var hourly = salaries
                .Where(x => x.Policy == PayrollPolicy.Hourly)
                .ToList();

            var prorated = salaries
                .Where(x => x.Policy == PayrollPolicy.ProRated)
                .ToList();

            return new SalarySummaryDTO(
                FullDayCount: fullDay.Count,
                FullDayTotal: fullDay.Sum(x => x.Rate),

                HalfDayCount: halfDay.Count,
                HalfDayTotal: halfDay.Sum(x => x.Rate),

                HourlyCount: hourly.Count,
                HourlyTotal: hourly.Sum(x => x.Rate),

                ProRatedCount: prorated.Count,
                ProRatedTotal: prorated.Sum(x => x.Rate)
            );
        }
    }
}
