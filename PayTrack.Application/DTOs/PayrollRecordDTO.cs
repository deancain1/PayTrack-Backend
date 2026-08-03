using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record PayrollRecordDTO
    (
        Guid Id,
        Guid EmployeeId,
        string fullName,
        string employeeNumber,
        string? departmentName,
        DateTime? TimeIn,
        DateTime? TimeOut,
        decimal hoursWorked,
        decimal amountEarned
        
    );
}
