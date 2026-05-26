using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Commands
{
    public record RegisterEmployeeCommand(
     string FullName,
     string Email,
     string? PhoneNumber,
     string Password,
     string Department,
    
     DateTime DateHired,
         string Role = "Employee"
 ) : IRequest<AuthResponseDTO>;
}
