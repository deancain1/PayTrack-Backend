using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class EmailOTP
    {
        public Guid Id { get; set; }

        public string ApplicationUserId { get; set; } = default!;

        public ApplicationUser ApplicationUser { get; set; } = default!;

        public string Code { get; set; } = default!;

        public EmailOTPPurpose Purpose { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public int FailedAttempts { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
