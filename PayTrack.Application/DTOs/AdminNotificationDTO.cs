using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.DTOs
{
    public  record AdminNotificationDTO(
     Guid Id,
     string Message,
     CompanyAdminNotificationType Type,
     DateTime CreatedAt,
     bool isRead
 );
}
