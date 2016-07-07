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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    DisplayName = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: false),
                    FamilyName = table.Column<string>(maxLength: 250, nullable: true),
                    GivenName = table.Column<string>(maxLength: 250, nullable: true),
                    Uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Child",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Gender = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    TotalScore = table.Column<int>(nullable: false),
                    Uid = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Child", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Child_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identifier",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Issuer = table.Column<string>(maxLength: 50, nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identifier_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ChildId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Tasks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.Id);
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
                    Task = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Score_Day_DayId",
                        column: x => x.DayId,
                        principalTable: "Day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Child_Uid",
                table: "Child",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Child_UserId",
                table: "Child",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Day_ChildId",
                table: "Day",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Day_ChildId_Date",
                table: "Day",
                columns: new[] { "ChildId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identifier_UserId",
                table: "Identifier",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identifier_Issuer_Value",
                table: "Identifier",
                columns: new[] { "Issuer", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Score_DayId",
                table: "Score",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_Score_DayId_Task",
                table: "Score",
                columns: new[] { "DayId", "Task" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Uid",
                table: "User",
                column: "Uid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Identifier");

            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "Child");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
