using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TerminalApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobApplicationModalityAndPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Modality",
                table: "JobApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "on-site");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "JobApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "C");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modality",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "JobApplications");
        }
    }
}
