using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TerminalApi.Data;

#nullable disable

namespace TerminalApi.Data.Migrations;

[DbContext(typeof(TerminalDbContext))]
[Migration("20261118120000_AddPriorityTasks")]
public partial class AddPriorityTasks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PriorityTasks",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Date = table.Column<string>(type: "TEXT", nullable: false),
                Position = table.Column<int>(type: "INTEGER", nullable: false),
                Text = table.Column<string>(type: "TEXT", nullable: false),
                Completed = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_PriorityTasks", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_PriorityTasks_Date_Position",
            table: "PriorityTasks",
            columns: new[] { "Date", "Position" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PriorityTasks");
    }
}
