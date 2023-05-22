using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManager.Migrations
{
    /// <inheritdoc />
    public partial class InitTenetDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tnt");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "tnt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "tnt");
        }
    }
}
