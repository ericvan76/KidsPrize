using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KidsPrize.Http.Migrations
{
    public partial class AddRedeem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_TaskGroup_ChildId",
            //     schema: "KidsPrize",
            //     table: "TaskGroup");

            // migrationBuilder.DropIndex(
            //     name: "IX_Score_ChildId",
            //     schema: "KidsPrize",
            //     table: "Score");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', 'public', ''")
                .OldAnnotation("Npgsql:PostgresExtension:public.uuid-ossp", "'uuid-ossp', 'public', ''");

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "TaskGroup",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "SortableTask",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "Score",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.CreateTable(
                name: "Redeem",
                schema: "KidsPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ChildId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redeem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Redeem_Child_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "KidsPrize",
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Redeem_ChildId_Timestamp",
                schema: "KidsPrize",
                table: "Redeem",
                columns: new[] { "ChildId", "Timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Redeem",
                schema: "KidsPrize");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:public.uuid-ossp", "'uuid-ossp', 'public', ''")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', 'public', ''");

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "TaskGroup",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "SortableTask",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            // migrationBuilder.AlterColumn<int>(
            //     name: "Id",
            //     schema: "KidsPrize",
            //     table: "Score",
            //     nullable: false,
            //     oldClrType: typeof(int))
            //     .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            // migrationBuilder.CreateIndex(
            //     name: "IX_TaskGroup_ChildId",
            //     schema: "KidsPrize",
            //     table: "TaskGroup",
            //     column: "ChildId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Score_ChildId",
            //     schema: "KidsPrize",
            //     table: "Score",
            //     column: "ChildId");
        }
    }
}
