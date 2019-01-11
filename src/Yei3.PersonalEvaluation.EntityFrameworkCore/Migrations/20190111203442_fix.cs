using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_EvaluationQuestions_NotEvaluableQuestionId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_NotEvaluableQuestionId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "NotEvaluableQuestionId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_EvaluationQuestionId2",
                table: "Answers",
                column: "EvaluationQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_EvaluationQuestions_EvaluationQuestionId2",
                table: "Answers",
                column: "EvaluationQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_EvaluationQuestions_EvaluationQuestionId2",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_EvaluationQuestionId2",
                table: "Answers");

            migrationBuilder.AddColumn<long>(
                name: "NotEvaluableQuestionId",
                table: "Answers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_NotEvaluableQuestionId",
                table: "Answers",
                column: "NotEvaluableQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_EvaluationQuestions_NotEvaluableQuestionId",
                table: "Answers",
                column: "NotEvaluableQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
