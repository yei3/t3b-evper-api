using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class BigShort : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationQuestions_Questions_EvaluationQuestionId1",
                table: "EvaluationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationQuestions_EvaluationQuestionId1",
                table: "EvaluationQuestions");

            migrationBuilder.AlterColumn<long>(
                name: "EvaluationQuestionId",
                table: "EvaluationQuestions",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "EvaluationUnmeasuredQuestion_EvaluationQuestionId",
                table: "EvaluationQuestions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationQuestions_EvaluationUnmeasuredQuestion_EvaluationQ~",
                table: "EvaluationQuestions",
                column: "EvaluationUnmeasuredQuestion_EvaluationQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationQuestions_Questions_EvaluationUnmeasuredQuestion_E~",
                table: "EvaluationQuestions",
                column: "EvaluationUnmeasuredQuestion_EvaluationQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationQuestions_Questions_EvaluationUnmeasuredQuestion_E~",
                table: "EvaluationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationQuestions_EvaluationUnmeasuredQuestion_EvaluationQ~",
                table: "EvaluationQuestions");

            migrationBuilder.DropColumn(
                name: "EvaluationUnmeasuredQuestion_EvaluationQuestionId",
                table: "EvaluationQuestions");

            migrationBuilder.AlterColumn<long>(
                name: "EvaluationQuestionId",
                table: "EvaluationQuestions",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationQuestions_EvaluationQuestionId1",
                table: "EvaluationQuestions",
                column: "EvaluationQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationQuestions_Questions_EvaluationQuestionId1",
                table: "EvaluationQuestions",
                column: "EvaluationQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
