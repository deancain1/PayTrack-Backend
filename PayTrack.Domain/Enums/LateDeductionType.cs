using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Enums
{
    public enum LateDeductionType
    {
        None = 1,
        FixedAmount = 2,
        Percentage = 3,
        PerMinute =  4
    }
}
