using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class missing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Section_SectionId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Section_Evaluations_EvaluationId",
                table: "Section");

            migrationBuilder.DropForeignKey(
                name: "FK_Section_Section_ParentId",
                table: "Section");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Section",
                table: "Section");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.RenameTable(
                name: "Section",
                newName: "Sections");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameIndex(
                name: "IX_Section_ParentId",
                table: "Sections",
                newName: "IX_Sections_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Section_EvaluationId",
                table: "Sections",
                newName: "IX_Sections_EvaluationId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_SectionId",
                table: "Questions",
                newName: "IX_Questions_SectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                table: "Sections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Sections_SectionId",
                table: "Questions",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Evaluations_EvaluationId",
                table: "Sections",
                column: "EvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Sections_ParentId",
                table: "Sections",
                column: "ParentId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Sections_SectionId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Evaluations_EvaluationId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Sections_ParentId",
                table: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                table: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "Sections",
                newName: "Section");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Question");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_ParentId",
                table: "Section",
                newName: "IX_Section_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_EvaluationId",
                table: "Section",
                newName: "IX_Section_EvaluationId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_SectionId",
                table: "Question",
                newName: "IX_Question_SectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Section",
                table: "Section",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Section_SectionId",
                table: "Question",
                column: "SectionId",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Section_Evaluations_EvaluationId",
                table: "Section",
                column: "EvaluationId",
                principalTable: "Evaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Section_Section_ParentId",
                table: "Section",
                column: "ParentId",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
