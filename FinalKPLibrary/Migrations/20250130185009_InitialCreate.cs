using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalKPLibrary.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Docs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Topic = table.Column<string>(type: "TEXT", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisibilityAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibilityAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocVisibilityAreas",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "INTEGER", nullable: false),
                    VisibilityAreaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocVisibilityAreas", x => new { x.DocId, x.VisibilityAreaId });
                    table.ForeignKey(
                        name: "FK_DocVisibilityAreas_Docs_DocId",
                        column: x => x.DocId,
                        principalTable: "Docs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocVisibilityAreas_VisibilityAreas_VisibilityAreaId",
                        column: x => x.VisibilityAreaId,
                        principalTable: "VisibilityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVisibilityAreas",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    VisibilityAreaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVisibilityAreas", x => new { x.UserId, x.VisibilityAreaId });
                    table.ForeignKey(
                        name: "FK_UserVisibilityAreas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVisibilityAreas_VisibilityAreas_VisibilityAreaId",
                        column: x => x.VisibilityAreaId,
                        principalTable: "VisibilityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocVisibilityAreas_VisibilityAreaId",
                table: "DocVisibilityAreas",
                column: "VisibilityAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVisibilityAreas_VisibilityAreaId",
                table: "UserVisibilityAreas",
                column: "VisibilityAreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocVisibilityAreas");

            migrationBuilder.DropTable(
                name: "UserVisibilityAreas");

            migrationBuilder.DropTable(
                name: "Docs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VisibilityAreas");
        }
    }
}
