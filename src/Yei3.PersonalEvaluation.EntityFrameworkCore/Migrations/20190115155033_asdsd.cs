using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class asdsd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_NotEvaluableQuestionId",
                table: "Binnacles");

            migrationBuilder.DropIndex(
                name: "IX_Binnacles_EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.DropIndex(
                name: "IX_Binnacles_NotEvaluableQuestionId",
                table: "Binnacles");

            migrationBuilder.DropColumn(
                name: "EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.DropColumn(
                name: "NotEvaluableQuestionId",
                table: "Binnacles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EvaluationMeasuredQuestionId",
                table: "Binnacles",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NotEvaluableQuestionId",
                table: "Binnacles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Binnacles_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                column: "EvaluationMeasuredQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Binnacles_NotEvaluableQuestionId",
                table: "Binnacles",
                column: "NotEvaluableQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                column: "EvaluationMeasuredQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_NotEvaluableQuestionId",
                table: "Binnacles",
                column: "NotEvaluableQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
