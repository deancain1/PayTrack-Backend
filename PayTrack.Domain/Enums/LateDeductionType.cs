using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Enums
{
    public enum LateDeductionType
    {
        None,

        FixedAmount,

        Percentage,

        PerMinute
    }
}
