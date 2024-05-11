using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CK_CSharp.Migrations
{
    /// <inheritdoc />
    public partial class fixSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_schedules_EmployeeId",
                table: "schedules",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_Employees_EmployeeId",
                table: "schedules",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedules_Employees_EmployeeId",
                table: "schedules");

            migrationBuilder.DropIndex(
                name: "IX_schedules_EmployeeId",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "schedules");
        }
    }
}
