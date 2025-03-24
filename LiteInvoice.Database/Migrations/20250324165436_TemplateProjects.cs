using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TemplateProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyRetainer",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MonthlyRetainerDescription",
                table: "Projects");

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "Projects");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyRetainer",
                table: "Projects",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MonthlyRetainerDescription",
                table: "Projects",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
