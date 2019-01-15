using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class binnacl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.AlterColumn<long>(
                name: "EvaluationMeasuredQuestionId",
                table: "Binnacles",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "EvaluationQuestionId",
                table: "Binnacles",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Binnacles_EvaluationQuestionId",
                table: "Binnacles",
                column: "EvaluationQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                column: "EvaluationMeasuredQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationQuestionId",
                table: "Binnacles",
                column: "EvaluationQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles");

            migrationBuilder.DropForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationQuestionId",
                table: "Binnacles");

            migrationBuilder.DropIndex(
                name: "IX_Binnacles_EvaluationQuestionId",
                table: "Binnacles");

            migrationBuilder.DropColumn(
                name: "EvaluationQuestionId",
                table: "Binnacles");

            migrationBuilder.AlterColumn<long>(
                name: "EvaluationMeasuredQuestionId",
                table: "Binnacles",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Binnacles_EvaluationQuestions_EvaluationMeasuredQuestionId",
                table: "Binnacles",
                column: "EvaluationMeasuredQuestionId",
                principalTable: "EvaluationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
