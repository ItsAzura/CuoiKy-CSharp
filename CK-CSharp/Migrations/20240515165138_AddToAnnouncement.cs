using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CK_CSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddToAnnouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "announcements",
                newName: "ImagePath");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "announcements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "announcements",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "announcements");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "announcements",
                newName: "Image");
        }
    }
}
