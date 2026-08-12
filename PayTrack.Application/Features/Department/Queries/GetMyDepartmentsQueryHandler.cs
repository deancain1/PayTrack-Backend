using AutoMapper;
using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Queries
{
    public class GetMyDepartmentsQueryHandler : IRequestHandler<GetMyDepartmentsQuery, List<DepartmentDTO>>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public GetMyDepartmentsQueryHandler(IDepartmentRepository departmentRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async  Task<List<DepartmentDTO>> Handle(GetMyDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _departmentRepository
                .GetDepartmentsByUserIdAsync(_currentUserService.UserId);

            return _mapper.Map<List<DepartmentDTO>>(departments);
        }
    }
}
