using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcarePortalApi.Migrations
{
    /// <inheritdoc />
    public partial class Add_AuditLog_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_DoctorId",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_OwningDoctorId",
                table: "AccessRequests",
                column: "OwningDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_PatientId",
                table: "AccessRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_RequestingDoctorId",
                table: "AccessRequests",
                column: "RequestingDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_Patients_PatientId",
                table: "AccessRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_Users_OwningDoctorId",
                table: "AccessRequests",
                column: "OwningDoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_Users_RequestingDoctorId",
                table: "AccessRequests",
                column: "RequestingDoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_DoctorId",
                table: "Patients",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_Patients_PatientId",
                table: "AccessRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_Users_OwningDoctorId",
                table: "AccessRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_Users_RequestingDoctorId",
                table: "AccessRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_DoctorId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_OwningDoctorId",
                table: "AccessRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_PatientId",
                table: "AccessRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_RequestingDoctorId",
                table: "AccessRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_DoctorId",
                table: "Patients",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
