using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Commands
{
    public class DeleteAttendancePolicyCommandHandler : IRequestHandler<DeleteAttendancePolicyCommand>
    {
        private readonly IAttendancePolicyRepository _attendancePolicyRepository;
        public DeleteAttendancePolicyCommandHandler(IAttendancePolicyRepository attendancePolicyRepository)
        {
            _attendancePolicyRepository = attendancePolicyRepository;
        }
        public async Task Handle(DeleteAttendancePolicyCommand request, CancellationToken cancellationToken)
        {
            var policy =
                await _attendancePolicyRepository.GetByIdAsync(request.Id);

            if (policy == null)
                throw new Exception("Attendance policy not found.");

            if (policy.IsDefault)
                throw new Exception("Default policy cannot be deleted.");

             _attendancePolicyRepository.Delete(policy);
        }
    }
    
}
