using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalKPLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Docs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Docs");
        }
    }
}
