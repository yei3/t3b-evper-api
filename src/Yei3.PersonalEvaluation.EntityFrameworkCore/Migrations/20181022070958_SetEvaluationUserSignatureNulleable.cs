using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class SetEvaluationUserSignatureNulleable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_UserSignatures_UserSignatureId",
                table: "Evaluations");

            migrationBuilder.AlterColumn<long>(
                name: "UserSignatureId",
                table: "Evaluations",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_UserSignatures_UserSignatureId",
                table: "Evaluations",
                column: "UserSignatureId",
                principalTable: "UserSignatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_UserSignatures_UserSignatureId",
                table: "Evaluations");

            migrationBuilder.AlterColumn<long>(
                name: "UserSignatureId",
                table: "Evaluations",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_UserSignatures_UserSignatureId",
                table: "Evaluations",
                column: "UserSignatureId",
                principalTable: "UserSignatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
