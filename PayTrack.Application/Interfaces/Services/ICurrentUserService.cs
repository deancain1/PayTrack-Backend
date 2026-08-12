using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
    }
}
