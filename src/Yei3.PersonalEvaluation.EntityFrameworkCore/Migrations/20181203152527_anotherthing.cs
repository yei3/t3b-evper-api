using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class anotherthing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequentQuestions_UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions",
                column: "UnmeasuredQuestion_SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FrequentQuestions_Sections_UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions",
                column: "UnmeasuredQuestion_SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrequentQuestions_Sections_UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions");

            migrationBuilder.DropIndex(
                name: "IX_FrequentQuestions_UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions");

            migrationBuilder.DropColumn(
                name: "UnmeasuredQuestion_SectionId",
                table: "FrequentQuestions");

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
    }
}
