﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CK_CSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeNameSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "schedules");
        }
    }
}
