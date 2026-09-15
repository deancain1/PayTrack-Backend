using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Notification.Commands;
using PayTrack.Application.Features.Notification.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

      
        [HttpGet]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> GetCompanyAdminNotifications()
        {
            return Ok(await _mediator.Send(
                new GetCompanyAdminNotificationsQuery()));
        }

       
        [HttpGet("unread-count")]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> GetCompanyAdminUnreadNotificationCount()
        {
            return Ok(await _mediator.Send(
                new GetCompanyAdminUnreadNotificationQuery()));
        }

        
        [HttpPut("{id}/read")]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _mediator.Send(
                new MarkNotificationAsReadCommand(id));

            return NoContent();
        }

       
        [HttpGet("employee-notifications")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeNotifications()
        {
            var result = await _mediator.Send(
                new GetEmployeeNotificationsQuery());

            return Ok(result);
        }

       
        [HttpGet("employee-unread-count")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeUnreadNotificationCount()
        {
            var result = await _mediator.Send(
                new GetUnreadEmployeeNotificationQuery());

            return Ok(result);
        }

       
        [HttpGet("employee-preferences")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeNotificationPreferences()
        {
            var result = await _mediator.Send(
                new GetEmployeeNotificationPreferencesQuery());

            return Ok(result);
        }

       
        [HttpPut("employee-preferences")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateEmployeeNotificationPreferences(
            [FromBody] UpdateEmployeeNotificationPreferencesCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}