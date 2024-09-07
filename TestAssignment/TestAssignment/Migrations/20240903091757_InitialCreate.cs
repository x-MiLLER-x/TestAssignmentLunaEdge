using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAssignment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                 name: "Tasks",
                 columns: table => new
                 {
                     Id = table.Column<Guid>(nullable: false),
                     Title = table.Column<string>(nullable: false),
                     Description = table.Column<string>(nullable: true),
                     DueDate = table.Column<DateTime>(nullable: true),
                     Status = table.Column<int>(nullable: false),
                     Priority = table.Column<int>(nullable: false),
                     CreatedAt = table.Column<DateTime>(nullable: false),
                     UpdatedAt = table.Column<DateTime>(nullable: false),
                     UserId = table.Column<Guid>(nullable: true)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Tasks", x => x.Id);
                     table.ForeignKey(
                         name: "FK_Tasks_Users_UserId",
                         column: x => x.UserId,
                         principalTable: "Users",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                 });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
