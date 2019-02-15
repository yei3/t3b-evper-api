using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class ActionCommitmentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommitmentDate",
                table: "Answers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitmentDate",
                table: "Answers");
        }
    }
}
