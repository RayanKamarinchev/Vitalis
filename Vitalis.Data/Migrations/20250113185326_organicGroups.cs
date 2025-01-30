using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vitalis.Data.Migrations
{
    /// <inheritdoc />
    public partial class organicGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Groups",
                table: "Tests");

            migrationBuilder.CreateTable(
                name: "OrganicGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganicGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestOrganicGroups",
                columns: table => new
                {
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganicGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestOrganicGroups", x => new { x.TestId, x.OrganicGroupId });
                    table.ForeignKey(
                        name: "FK_TestOrganicGroups_OrganicGroups_OrganicGroupId",
                        column: x => x.OrganicGroupId,
                        principalTable: "OrganicGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestOrganicGroups_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestOrganicGroups_OrganicGroupId",
                table: "TestOrganicGroups",
                column: "OrganicGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestOrganicGroups");

            migrationBuilder.DropTable(
                name: "OrganicGroups");

            migrationBuilder.AddColumn<string>(
                name: "Groups",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
