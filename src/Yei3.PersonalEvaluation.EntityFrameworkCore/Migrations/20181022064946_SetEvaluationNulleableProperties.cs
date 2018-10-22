using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class SetEvaluationNulleableProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccomplished",
                table: "Objectives");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "AbpUsers",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbpUsers",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccomplished",
                table: "Objectives",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "AbpUsers",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbpUsers",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);
        }
    }
}
