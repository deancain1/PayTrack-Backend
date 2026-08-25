using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Commands
{
    public record UpdateEmployeeCommand(
     Guid Id,

     // AspNetUsers
     string FullName,
     string Email,
     string PhoneNumber,

     Guid DepartmentId
 ) : IRequest<EmployeeDTO>;
}
