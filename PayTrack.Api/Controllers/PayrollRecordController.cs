using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.PayrollRecords.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayrollRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PayrollRecordDTO>>> GetParollRecordByEmployeeAsync([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 7)
        {
            var query = new GetPayrollRecordsByCurrentEmployeeQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("payroll-records-by-company-admin")]
        public async Task<IActionResult> GetPayrollRecordByCompanyAdminAsync([FromQuery] GetPayrollRecordsByCompanyAdminQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("total-earnings")]
        public async Task<IActionResult> GetTotalEarnings()
        {
            var result = await _mediator.Send(new GetEmployeeTotalEarningsQuery());

            return Ok(result);
        }

        [HttpGet("payroll-record-summary")]
        public async Task<ActionResult> GetDashboardSummary()
        {
            var result = await _mediator.Send(
                new GetPayrollRecordSummaryQuery());

            return Ok(result);
        }
    }
}
