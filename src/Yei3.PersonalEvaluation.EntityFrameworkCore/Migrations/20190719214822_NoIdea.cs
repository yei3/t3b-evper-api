using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class NoIdea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Expected",
                table: "EvaluationQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedText",
                table: "EvaluationQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expected",
                table: "EvaluationQuestions");

            migrationBuilder.DropColumn(
                name: "ExpectedText",
                table: "EvaluationQuestions");
        }
    }
}
