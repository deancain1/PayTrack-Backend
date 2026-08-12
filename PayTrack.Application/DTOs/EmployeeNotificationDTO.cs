using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public record EmployeeNotificationDTO
    (
        Guid Id,
        EmployeeNotificationType Type,
        string Title,
        string Message,
        bool IsRead,
        DateTime CreatedAt
    );
}
