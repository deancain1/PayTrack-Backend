using Microsoft.Extensions.DependencyInjection;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Infrastructure.Repository;
using PayTrack.Infrastructure.Services;

namespace PayTrack.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddHttpClient<IEmailService, EmailService>();
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
            services.AddScoped<IAttendanceAutomationService, AttendanceAutomationService>();
            services.AddHostedService<AttendanceBackgroundService>();

            services.AddHostedService<TimeOutReminderBackgroundService>();
            services.AddScoped<IAttendancePolicyRepository, AttendancePolicyRepository>();
            services.AddScoped<ISalaryAdvanceRepository, SalaryAdvanceRepository>();
            services.AddScoped<IEmailOTPRepository, EmailOTPRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyPayrollSettingRepository, CompanyPayrollSettingRepository>();
            services.AddScoped<ISalaryAdvanceEligibilityService, SalaryAdvanceEligibilityService>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<ICompanyDetailsRepository, CompanyDetailsRepository>();
            services.AddScoped<IOvertimeRepository, OvertimeRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            //notifications
            services.AddScoped<ICompanyAdminNotificationService, CompanyAdminNotificationService>();
            services.AddScoped<ICompanyAdminNotificationRepository, CompanyAdminNotificationRepository>();
            services.AddScoped<IEmployeeNotificationRepository, EmployeeNotificationRepository>();
            services.AddScoped<IEmployeeNotificationService, EmployeeNotificationService>();
            services.AddScoped<IEmployeeNotificationPreferenceRepository, EmployeeNotificationPreferenceRepository>();


            //taxes
            services.AddScoped<ISSSContributionRepository, SSSContributionRepository>();
            services.AddScoped<ISSSService, SSSService>();
            services.AddScoped<IPhilHealthContributionRepository, PhilHealthContributionRepository>();
            services.AddScoped<IPhilHealthService, PhilHealthService>();
            services.AddScoped<IPagIbigContributionRepository, PagIbigContributionRepository>();
            services.AddScoped<IPagIbigService, PagIbigService>();
            return services;
        }
    }
}
