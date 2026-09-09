using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class PasswordResetOTP
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? Email { get; set; }

        public string OtpCode { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
