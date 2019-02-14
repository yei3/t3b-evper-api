using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class update_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Answers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnmeasuredAnswer_CommitmentTime",
                table: "Answers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "UnmeasuredAnswer_CommitmentTime",
                table: "Answers");
        }
    }
}
