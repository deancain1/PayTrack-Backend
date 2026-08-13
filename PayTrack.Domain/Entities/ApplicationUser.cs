using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public string Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public byte[]? ProfilePicture { get; set; }
        
        public Employees? Employee { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Departments> Departments { get; set; }
         = new List<Departments>();

        public bool TwoFactorEnabled { get; set; }

        public ICollection<EmailOTP> EmailOtps { get; set; }
            = new List<EmailOTP>();

        public CompanyDetails? CompanyDetails { get; set; }
    }
}
