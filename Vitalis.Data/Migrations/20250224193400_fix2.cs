using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitalis.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "OpenQuestionAnswers",
                newName: "UserAnswer");

            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "OpenQuestionAnswers",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "OpenQuestionAnswers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
