using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CK_CSharp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeImgTypeSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "schedules",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "schedules",
                newName: "Image");
        }
    }
}
