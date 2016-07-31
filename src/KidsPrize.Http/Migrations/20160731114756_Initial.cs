using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsPrize.Http.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreatePostgresExtension(name: "uuid-ossp",
                schema: "public");

            migrationBuilder.EnsureSchema(
                name: "KidsPrize");

            migrationBuilder.CreateTable(
                name: "Child",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
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
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
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
                        principalSchema: "KidsPrize",
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Score",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
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
                        principalSchema: "KidsPrize",
                        principalTable: "Day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Child_UserId",
                schema: "KidsPrize",
                table: "Child",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Day_ChildId",
                schema: "KidsPrize",
                table: "Day",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Score_DayId",
                schema: "KidsPrize",
                table: "Score",
                column: "DayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPostgresExtension("uuid-ossp");

            migrationBuilder.DropTable(
                name: "Score",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "Day",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "Child",
                schema: "KidsPrize");
        }
    }
}
