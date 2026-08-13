using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Enums
{
    public enum  EmailOTPPurpose
    {
        Login = 1,
        ForgotPassword = 2,
        EmailVerification = 3,
        ChangeEmail = 4
    }
}
