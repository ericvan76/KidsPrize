using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsPrize.Http.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Child",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Gender = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    TotalScore = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Child", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChildId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.Id);
                    table.UniqueConstraint("AK_Day_ChildId_Date", x => new { x.ChildId, x.Date });
                    table.ForeignKey(
                        name: "FK_Day_Child_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Score",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    DayId = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Task = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => x.Id);
                    table.UniqueConstraint("AK_Score_DayId_Task", x => new { x.DayId, x.Task });
                    table.ForeignKey(
                        name: "FK_Score_Day_DayId",
                        column: x => x.DayId,
                        principalTable: "Day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Child_UserId",
                table: "Child",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Day_ChildId",
                table: "Day",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Score_DayId",
                table: "Score",
                column: "DayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "Child");
        }
    }
}
