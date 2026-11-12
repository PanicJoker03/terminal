using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TerminalApi.Data;

#nullable disable

namespace TerminalApi.Data.Migrations;

[DbContext(typeof(TerminalDbContext))]
[Migration("20261112150000_AddJobApplicationContactRhNameAndChannel")]
public partial class AddJobApplicationContactRhNameAndChannel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Contact",
            table: "JobApplications",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RhName",
            table: "JobApplications",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Channel",
            table: "JobApplications",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Contact", table: "JobApplications");
        migrationBuilder.DropColumn(name: "RhName", table: "JobApplications");
        migrationBuilder.DropColumn(name: "Channel", table: "JobApplications");
    }
}
