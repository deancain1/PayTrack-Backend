using Microsoft.Extensions.DependencyInjection;
using PayTrack.Application.Interfaces;
using PayTrack.Infrastructure.Repository;
using PayTrack.Infrastructure.Services;

namespace PayTrack.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IQrCodeService, QRCodeService>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IPhotoStorageService, PhotoStorageService>();
            services.AddScoped<ISalaryRepository, SalaryRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            services.AddScoped<IPayrollService, PayrollService>();
            services.AddScoped<IPayrollRecordRepository, PayrollRecordRepository>();
            services.AddScoped<IPayrollBatchRepository, PayrollBatchRepository>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
            services.AddScoped<IAttendancePolicyService, AttendancePolicyService>();
            return services;
        }
    }
}
