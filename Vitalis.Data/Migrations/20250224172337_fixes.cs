using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitalis.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_ClosedQuestions_QuestionId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "Answers",
                newName: "ClosedQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                newName: "IX_Answers_ClosedQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_ClosedQuestions_ClosedQuestionId",
                table: "Answers",
                column: "ClosedQuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_ClosedQuestions_ClosedQuestionId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "ClosedQuestionId",
                table: "Answers",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_ClosedQuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_ClosedQuestions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
