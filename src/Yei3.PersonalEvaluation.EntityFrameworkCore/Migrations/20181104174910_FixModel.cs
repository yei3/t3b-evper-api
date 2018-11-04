using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class FixModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCapabilities_EvaluationUsers_UserId",
                table: "UserCapabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserObjectives_EvaluationUsers_UserId",
                table: "UserObjectives");

            migrationBuilder.DropIndex(
                name: "IX_UserObjectives_UserId",
                table: "UserObjectives");

            migrationBuilder.DropIndex(
                name: "IX_UserCapabilities_UserId",
                table: "UserCapabilities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserObjectives_UserId",
                table: "UserObjectives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCapabilities_UserId",
                table: "UserCapabilities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCapabilities_EvaluationUsers_UserId",
                table: "UserCapabilities",
                column: "UserId",
                principalTable: "EvaluationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserObjectives_EvaluationUsers_UserId",
                table: "UserObjectives",
                column: "UserId",
                principalTable: "EvaluationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
