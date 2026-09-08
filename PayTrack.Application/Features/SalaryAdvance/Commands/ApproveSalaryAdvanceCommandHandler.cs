using MediatR;
using PayTrack.Application.Features.Notification.Events;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{

    public class ApproveSalaryAdvanceCommandHandler: IRequestHandler<ApproveSalaryAdvanceCommand, bool>
    {
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPublisher _publisher;

        public ApproveSalaryAdvanceCommandHandler(
            ISalaryAdvanceRepository salaryAdvanceRepository, IPayrollRepository payrollRepository, IPublisher publisher)
        {
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _payrollRepository = payrollRepository;
            _publisher = publisher;
        }

        public async Task<bool> Handle(ApproveSalaryAdvanceCommand request, CancellationToken cancellationToken)
        {
           
            var salaryAdvance = await _salaryAdvanceRepository.GetByIdAsync(request.Id);

            if (salaryAdvance == null)
            {
                throw new Exception("Salary advance request not found.");
            }

         
            if (salaryAdvance.Status != SalaryAdvanceStatus.Pending)
            {
                throw new Exception("Only pending requests can be approved.");
            }

            // 3. Check if payroll has already been generated for the request date
            var payrollExists = await _payrollRepository.HasGeneratedPayrollForDateAsync(
                salaryAdvance.EmployeeId,
                salaryAdvance.RequestDate);

            if (payrollExists)
            {
                throw new Exception(
                    "This salary advance cannot be approved because payroll has already been generated for this period.");
            }

           
            salaryAdvance.Status = SalaryAdvanceStatus.Approved;

             _salaryAdvanceRepository.Update(salaryAdvance);
           await _salaryAdvanceRepository.SaveChangesAsync();

            await _publisher.Publish(
              new EmployeeNotificationCreatedEvent(
                  NotificationId: Guid.NewGuid(),
                  ApplicationUserId: salaryAdvance.ApplicationUserId,
                  Type: EmployeeNotificationType.SalaryAdvanceApproved,
                  Title: "Salary Advance Approved",
                  Message: $"Your salary advance request of ₱{salaryAdvance.Amount:N2} has been approved."
              ),
              cancellationToken);

            return true;
        }

    }

}