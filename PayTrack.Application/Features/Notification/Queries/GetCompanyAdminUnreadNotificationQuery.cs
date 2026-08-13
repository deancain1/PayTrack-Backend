using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Notification.Queries
{
    public record GetCompanyAdminUnreadNotificationQuery : IRequest<int>
    {
    }
}
