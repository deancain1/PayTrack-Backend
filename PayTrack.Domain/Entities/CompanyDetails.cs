using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class CompanyDetails
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyEmail { get; set; }
        public string? CompanyContactNumber { get; set; }
        public string? CompanyAddress { get; set; }
        public string? BusinessType { get; set; }

        public byte[]? CompanyLogo { get; set; }
        public string CompanyAdminId { get; set; } = string.Empty;
        public ApplicationUser ApplicatonUser { get; set; } = null!;
    }
}
