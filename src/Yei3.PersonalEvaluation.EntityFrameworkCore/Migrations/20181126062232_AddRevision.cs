using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yei3.PersonalEvaluation.Migrations
{
    public partial class AddRevision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EvaluationQuestions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "EvaluationId",
                table: "EvaluationAnswer",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "EvaluationRevisions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    EvaluationId = table.Column<long>(nullable: false),
                    ReviewerUserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationRevisions_AbpUsers_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationAnswer_EvaluationId",
                table: "EvaluationAnswer",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationRevisions_ReviewerUserId",
                table: "EvaluationRevisions",
                column: "ReviewerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_Id",
                table: "Evaluation",
                column: "Id",
                principalTable: "EvaluationRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationAnswer_Evaluation_EvaluationId",
                table: "EvaluationAnswer",
                column: "EvaluationId",
                principalTable: "Evaluation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluation_EvaluationRevisions_Id",
                table: "Evaluation");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationAnswer_Evaluation_EvaluationId",
                table: "EvaluationAnswer");

            migrationBuilder.DropTable(
                name: "EvaluationRevisions");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationAnswer_EvaluationId",
                table: "EvaluationAnswer");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EvaluationQuestions");

            migrationBuilder.DropColumn(
                name: "EvaluationId",
                table: "EvaluationAnswer");
        }
    }
}
