using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitalis.Data.Migrations
{
    /// <inheritdoc />
    public partial class closedQuestionAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "UserAnswersIndexes",
            //    table: "ClosedQuestionAnswers");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClosedQuestionAnswerSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosedQuestionAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedQuestionAnswerSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClosedQuestionAnswerSelections_ClosedQuestionAnswers_Closed~",
                        column: x => x.ClosedQuestionAnswerId,
                        principalTable: "ClosedQuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestionAnswerSelections_ClosedQuestionAnswerId",
                table: "ClosedQuestionAnswerSelections",
                column: "ClosedQuestionAnswerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClosedQuestionAnswerSelections");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "UserAnswersIndexes",
                table: "ClosedQuestionAnswers",
                type: "text",
                nullable: true);
        }
    }
}
