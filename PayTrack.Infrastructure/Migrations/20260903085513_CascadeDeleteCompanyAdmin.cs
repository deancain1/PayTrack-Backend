using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteCompanyAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendancePolicies_AspNetUsers_CompanyAdminId",
                table: "AttendancePolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDetails_AspNetUsers_CompanyAdminId",
                table: "CompanyDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_CompanyAdminId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollRecords_AspNetUsers_ApplicationUserId",
                table: "PayrollRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_AspNetUsers_ApplicationUserId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_Employees_EmployeeId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_Payrolls_PayrollId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendancePolicies_AspNetUsers_CompanyAdminId",
                table: "AttendancePolicies",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDetails_AspNetUsers_CompanyAdminId",
                table: "CompanyDetails",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_CompanyAdminId",
                table: "LeaveRequests",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollRecords_AspNetUsers_ApplicationUserId",
                table: "PayrollRecords",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_AspNetUsers_ApplicationUserId",
                table: "SalaryAdvances",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_Employees_EmployeeId",
                table: "SalaryAdvances",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_Payrolls_PayrollId",
                table: "SalaryAdvances",
                column: "PayrollId",
                principalTable: "Payrolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendancePolicies_AspNetUsers_CompanyAdminId",
                table: "AttendancePolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDetails_AspNetUsers_CompanyAdminId",
                table: "CompanyDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_CompanyAdminId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollRecords_AspNetUsers_ApplicationUserId",
                table: "PayrollRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_AspNetUsers_ApplicationUserId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_Employees_EmployeeId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryAdvances_Payrolls_PayrollId",
                table: "SalaryAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendancePolicies_AspNetUsers_CompanyAdminId",
                table: "AttendancePolicies",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDetails_AspNetUsers_CompanyAdminId",
                table: "CompanyDetails",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_CompanyAdminId",
                table: "LeaveRequests",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollRecords_AspNetUsers_ApplicationUserId",
                table: "PayrollRecords",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_AspNetUsers_ApplicationUserId",
                table: "SalaryAdvances",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_Employees_EmployeeId",
                table: "SalaryAdvances",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryAdvances_Payrolls_PayrollId",
                table: "SalaryAdvances",
                column: "PayrollId",
                principalTable: "Payrolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
