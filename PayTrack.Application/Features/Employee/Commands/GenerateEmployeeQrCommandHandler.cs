using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class GenerateEmployeeQrCommandHandler : IRequestHandler<GenerateEmployeeQrCommand, GenerateQrResponseDTO>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IQrCodeService _qrCodeService;

        public GenerateEmployeeQrCommandHandler(
            IEmployeesRepository employeesRepository,
            IQrCodeService qrCodeService)
        {
            _employeesRepository = employeesRepository;
            _qrCodeService = qrCodeService;
        }

        public async Task<GenerateQrResponseDTO> Handle(
        GenerateEmployeeQrCommand request,
        CancellationToken cancellationToken)
        {
            var employee =
                await _employeesRepository.GetEmployeeByEmployeeIdAsync(request.EmployeeId);

            if (employee is null)
                throw new Exception("Employee not found");

            employee.QrCodeBase64 = _qrCodeService.GenerateQrCodeBase64($"{employee.Id}|{employee.EmployeeNumber}");

            await _employeesRepository.UpdateAsync(employee);

            return new GenerateQrResponseDTO(
                employee.Id,
                employee.EmployeeNumber,
                employee.QrCodeBase64
            );
        }
    }
}
