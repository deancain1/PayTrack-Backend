using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Employee.Commands;
using PayTrack.Application.Features.Employee.Queries;


namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/generate-qr")]
        public async Task<IActionResult> GenerateQr(Guid id)
        {
            var result = await _mediator.Send(
                new GenerateEmployeeQrCommand(id));

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
