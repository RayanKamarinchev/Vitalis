using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitalis.Data.Migrations
{
    /// <inheritdoc />
    public partial class compounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_ClosedQuestions_QuestionId",
                table: "Answer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Compounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhysicalAppearance = table.Column<string>(type: "text", nullable: false),
                    Applications = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compounds", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_ClosedQuestions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_ClosedQuestions_QuestionId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "Compounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answer",
                newName: "IX_Answer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_ClosedQuestions_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
