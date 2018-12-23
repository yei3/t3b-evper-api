using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class AddSexToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_EvaluationQuestionId1",
                table: "Answers");

            migrationBuilder.AddColumn<bool>(
                name: "IsMale",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_EvaluationQuestionId1",
                table: "Answers",
                column: "EvaluationQuestionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_EvaluationQuestionId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IsMale",
                table: "AbpUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_EvaluationQuestionId1",
                table: "Answers",
                column: "EvaluationQuestionId");
        }
    }
}
