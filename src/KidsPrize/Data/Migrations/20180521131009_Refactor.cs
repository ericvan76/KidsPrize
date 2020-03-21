using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace KidsPrize.Data.Migrations
{
    public partial class Refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', 'public', ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', 'public', ''")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''");
        }
    }
}
