using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{

    public class ApproveSalaryAdvanceCommandHandler: IRequestHandler<ApproveSalaryAdvanceCommand, bool>
    {
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;

        public ApproveSalaryAdvanceCommandHandler(
            ISalaryAdvanceRepository salaryAdvanceRepository)
        {
            _salaryAdvanceRepository = salaryAdvanceRepository;
        }

        public async Task<bool> Handle(ApproveSalaryAdvanceCommand request, CancellationToken cancellationToken)
        {

            var salaryAdvance =
                await _salaryAdvanceRepository
                .GetByIdAsync(request.Id);


            if (salaryAdvance == null)
            {
                throw new Exception(
                    "Salary advance request not found.");
            }


            if (salaryAdvance.Status != SalaryAdvanceStatus.Pending)
            {
                throw new Exception(
                    "Only pending requests can be approved.");
            }



            salaryAdvance.Status =
                SalaryAdvanceStatus.Approved;



            await _salaryAdvanceRepository
                .UpdateAsync(salaryAdvance);



            return true;

        }

    }

}