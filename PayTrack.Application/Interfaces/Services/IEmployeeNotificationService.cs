using PayTrack.Application.DTOs;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Services
{
    public interface  IEmployeeNotificationService
    {
        Task SendToUserAsync(string applicationUserId, EmployeeNotificationDTO notification);
    }
}

