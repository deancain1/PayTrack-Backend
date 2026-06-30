using MediatR;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public class AssignScheduleToEmployeeCommandHandler : IRequestHandler<AssignScheduleToEmployeeCommand>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        public AssignScheduleToEmployeeCommandHandler(IWorkScheduleRepository workScheduleRepository)
        {
            _workScheduleRepository = workScheduleRepository;
        }
        public async Task Handle(AssignScheduleToEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _workScheduleRepository.AssignScheduleToEmployeeAsync(
                request.EmployeeId, request.WorkScheduleId, request.ScheduleEffectiveDate);
        }
    }
}
