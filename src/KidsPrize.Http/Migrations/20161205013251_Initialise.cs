using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsPrize.Http.Migrations
{
    public partial class Initialise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "KidsPrize");

            migrationBuilder.EnsurePostgresExtension(name: "uuid-ossp",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "Child",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Gender = table.Column<string>(maxLength: 10, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    TotalScore = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Child", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preference",
                schema: "KidsPrize",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 250, nullable: false),
                    TimeZoneOffset = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preference", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Score",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    ChildId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Task = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => x.Id);
                    table.UniqueConstraint("AK_Score_ChildId_Date_Task", x => new { x.ChildId, x.Date, x.Task });
                    table.ForeignKey(
                        name: "FK_Score_Child_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "KidsPrize",
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskGroup",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    ChildId = table.Column<Guid>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskGroup", x => x.Id);
                    table.UniqueConstraint("AK_TaskGroup_ChildId_EffectiveDate", x => new { x.ChildId, x.EffectiveDate });
                    table.ForeignKey(
                        name: "FK_TaskGroup_Child_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "KidsPrize",
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SortableTask",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    TaskGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortableTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortableTask_TaskGroup_TaskGroupId",
                        column: x => x.TaskGroupId,
                        principalSchema: "KidsPrize",
                        principalTable: "TaskGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Child_UserId",
                schema: "KidsPrize",
                table: "Child",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Score_ChildId",
                schema: "KidsPrize",
                table: "Score",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_SortableTask_TaskGroupId",
                schema: "KidsPrize",
                table: "SortableTask",
                column: "TaskGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskGroup_ChildId",
                schema: "KidsPrize",
                table: "TaskGroup",
                column: "ChildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preference",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "Score",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "SortableTask",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "TaskGroup",
                schema: "KidsPrize");

            migrationBuilder.DropTable(
                name: "Child",
                schema: "KidsPrize");
        }
    }
}
