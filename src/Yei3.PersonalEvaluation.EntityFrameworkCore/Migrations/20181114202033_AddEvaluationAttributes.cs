using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class AddEvaluationAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Evaluations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Evaluations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Evaluations");
        }
    }
}
