using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public  class CompanyPayrollSetting
    {
        public Guid Id { get; set; }
        public string CompanyAdminId { get; set; } = string.Empty;
        public PayrollFrequency Frequency { get; set; }


    }
}
