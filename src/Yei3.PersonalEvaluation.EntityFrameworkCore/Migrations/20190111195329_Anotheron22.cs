using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class Anotheron22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SectionId",
                table: "EvaluationQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "EvaluationQuestions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CommitmentTime",
                table: "Answers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NotEvaluableQuestionId",
                table: "Answers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationQuestions_SectionId",
                table: "EvaluationQuestions",
                column: "SectionId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationQuestions_Sections_SectionId",
                table: "EvaluationQuestions",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_EvaluationQuestions_NotEvaluableQuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationQuestions_Sections_SectionId",
                table: "EvaluationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationQuestions_SectionId",
                table: "EvaluationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_NotEvaluableQuestionId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "EvaluationQuestions");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "EvaluationQuestions");

            migrationBuilder.DropColumn(
                name: "CommitmentTime",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "NotEvaluableQuestionId",
                table: "Answers");
        }
    }
}
