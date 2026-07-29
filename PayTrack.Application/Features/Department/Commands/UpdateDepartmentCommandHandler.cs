using MediatR;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Commands
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository, ICurrentUserService currentUserService)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            var department = await _departmentRepository.GetByIdAndCompanyAdminAsync(request.Id, currentUserId);

            if (department == null)
                throw new Exception("No found Department");

            department.DepartmentName = request.departmentName;
            department.Description = request.description;

             _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();
            return true;
        }
    }
}
