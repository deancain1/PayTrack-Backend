using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Department.Commands;
using PayTrack.Application.Features.Department.Queries;
using PayTrack.Application.Features.WorkSchedule.Commands;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("department")]
        public async Task<IActionResult> CreatedDepartment([FromBody] CreateDepartmentCommand cmd)
        {
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments(int pageNumber = 1, int? pageSize = null, string? search = null)
        {
            var result = await _mediator.Send(
                new GetDepartmentsQuery(pageNumber, pageSize, search)
            );

            return Ok(result);
        }

        [HttpGet("my-departments")]
        public async Task<IActionResult> GetMyDepartments()
        {
            var departments = await _mediator.Send(new GetMyDepartmentsQuery());

            return Ok(departments);

        }

        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] DeleteDepartmentsCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
