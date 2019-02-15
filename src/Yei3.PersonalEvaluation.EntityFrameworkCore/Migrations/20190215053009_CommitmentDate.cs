using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class CommitmentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnmeasuredAnswer_CommitmentTime",
                table: "Answers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CommitmentDate",
                table: "Answers",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CommitmentDate",
                table: "Answers",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnmeasuredAnswer_CommitmentTime",
                table: "Answers",
                nullable: true);
        }
    }
}
