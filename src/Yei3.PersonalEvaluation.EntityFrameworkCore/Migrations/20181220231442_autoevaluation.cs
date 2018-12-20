using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class autoevaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoEvaluation",
                table: "EvaluationTemplates",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoEvaluation",
                table: "EvaluationTemplates");
        }
    }
}
