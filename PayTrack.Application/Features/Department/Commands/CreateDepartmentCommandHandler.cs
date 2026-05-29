using AutoMapper;
using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Commands
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDTO>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<DepartmentDTO> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = _mapper.Map<Departments>(request);
            department.ApplicationUserId = _currentUserService.UserId;
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return _mapper.Map<DepartmentDTO>(department);
        }
    }
}
