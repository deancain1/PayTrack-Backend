using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Company.Command;
using PayTrack.Application.Features.Company.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("company-details")]
        public async Task<IActionResult> GetCompanyDetailsAsync()
        {
            var companyDetails = await _mediator.Send(
                new GetCompanyDetailsQuery());

            return Ok(companyDetails);
        }

        [HttpPut("company-details")]
        public async Task<ActionResult<CompanyDetailsDTO>> UpdateDetails(UpdateCompanyDetailsCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}