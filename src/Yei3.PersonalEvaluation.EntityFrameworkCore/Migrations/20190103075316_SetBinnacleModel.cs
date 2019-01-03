using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class SetBinnacleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveBinnacle_EvaluationQuestions_EvaluationMeasuredQues~",
                table: "ObjectiveBinnacle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjectiveBinnacle",
                table: "ObjectiveBinnacle");

            migrationBuilder.RenameTable(
                name: "ObjectiveBinnacle",
                newName: "Binnacles");

            migrationBuilder.RenameIndex(
                name: "IX_ObjectiveBinnacle_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                newName: "IX_Binnacles_EvaluationMeasuredQuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Binnacles",
                table: "Binnacles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                column: "EvaluationMeasuredQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Binnacles",
                table: "Binnacles");

            migrationBuilder.RenameTable(
                name: "Binnacles",
                newName: "ObjectiveBinnacle");

            migrationBuilder.RenameIndex(
                name: "IX_Binnacles_EvaluationMeasuredQuestionId",
                table: "ObjectiveBinnacle",
                newName: "IX_ObjectiveBinnacle_EvaluationMeasuredQuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjectiveBinnacle",
                table: "ObjectiveBinnacle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveBinnacle_EvaluationQuestions_EvaluationMeasuredQues~",
                table: "ObjectiveBinnacle",
                column: "EvaluationMeasuredQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
