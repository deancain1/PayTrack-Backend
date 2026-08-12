using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class EmployeeNotification
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; }
        public EmployeeNotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; } 
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
