using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Section");

            migrationBuilder.AddColumn<bool>(
                name: "ShowName",
                table: "Section",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowName",
                table: "Section");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Section",
                nullable: true);
        }
    }
}
