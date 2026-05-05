using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boi.Net.Migrations
{
    /// <inheritdoc />
    public partial class BookModelUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Books",
                newName: "CoverPublicId");

            migrationBuilder.AddColumn<string>(
                name: "CoverPhoto",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPhoto",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "CoverPublicId",
                table: "Books",
                newName: "ImageUrl");
        }
    }
}
