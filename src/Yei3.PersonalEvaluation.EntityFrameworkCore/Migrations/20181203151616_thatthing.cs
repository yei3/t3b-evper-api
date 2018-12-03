using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class thatthing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SectionId1",
                table: "FrequentQuestions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequentQuestions_SectionId1",
                table: "FrequentQuestions",
                column: "SectionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FrequentQuestions_Sections_SectionId1",
                table: "FrequentQuestions",
                column: "SectionId1",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrequentQuestions_Sections_SectionId1",
                table: "FrequentQuestions");

            migrationBuilder.DropIndex(
                name: "IX_FrequentQuestions_SectionId1",
                table: "FrequentQuestions");

            migrationBuilder.DropColumn(
                name: "SectionId1",
                table: "FrequentQuestions");
        }
    }
}
