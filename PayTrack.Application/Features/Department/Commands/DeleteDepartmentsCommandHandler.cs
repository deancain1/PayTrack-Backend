using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Commands
{
    public class DeleteDepartmentsCommandHandler : IRequestHandler<DeleteDepartmentsCommand, Unit>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteDepartmentsCommandHandler(IDepartmentRepository departmentRepository, IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
        }
        public async Task<Unit> Handle(DeleteDepartmentsCommand request, CancellationToken cancellationToken)
        {
            var departments = await _departmentRepository.GetByIdsAsync(request.DepartmentIds);

            if (!departments.Any())
                throw new Exception("No departments found.");

            var currentUserId = _currentUserService.UserId;

            // Example: only the company admin who owns this department can delete it
            if (departments.Any(d => d.ApplicationUserId != currentUserId))
                throw new UnauthorizedAccessException(
                    "You are not authorized to delete one or more selected departments.");

            if (await _employeesRepository.HasEmployeesInDepartmentsAsync(request.DepartmentIds))
            {
                throw new Exception(
                    "One or more selected departments have employees assigned. Please reassign or remove those employees first."
                );
            }
            await  _departmentRepository.DeleteRangeAsync(departments);
            return Unit.Value;
        }
    }
}
