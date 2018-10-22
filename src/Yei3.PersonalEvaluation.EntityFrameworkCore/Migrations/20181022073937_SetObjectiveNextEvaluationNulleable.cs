using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class SetObjectiveNextEvaluationNulleable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_Evaluations_NextEvaluationId",
                table: "Objectives");

            migrationBuilder.AlterColumn<long>(
                name: "NextEvaluationId",
                table: "Objectives",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_Evaluations_NextEvaluationId",
                table: "Objectives",
                column: "NextEvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_Evaluations_NextEvaluationId",
                table: "Objectives");

            migrationBuilder.AlterColumn<long>(
                name: "NextEvaluationId",
                table: "Objectives",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_Evaluations_NextEvaluationId",
                table: "Objectives",
                column: "NextEvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
