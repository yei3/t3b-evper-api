using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class addRevisionForeingKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_Id",
                table: "Evaluation");

            migrationBuilder.AddColumn<long>(
                name: "RevisionId",
                table: "Evaluation",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_RevisionId",
                table: "Evaluation",
                column: "RevisionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_RevisionId",
                table: "Evaluation",
                column: "RevisionId",
                principalTable: "EvaluationRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_RevisionId",
                table: "Evaluation");

            migrationBuilder.DropIndex(
                name: "IX_Evaluation_RevisionId",
                table: "Evaluation");

            migrationBuilder.DropColumn(
                name: "RevisionId",
                table: "Evaluation");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_Id",
                table: "Evaluation",
                column: "Id",
                principalTable: "EvaluationRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
