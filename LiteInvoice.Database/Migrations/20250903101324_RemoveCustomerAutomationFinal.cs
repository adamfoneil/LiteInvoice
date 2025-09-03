using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerAutomationFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Projects_AutoPostTemplateId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AutoPostTemplateId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AutoPostDayOfMonth",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AutoPostTemplateId",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoPostDayOfMonth",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AutoPostTemplateId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AutoPostTemplateId",
                table: "Customers",
                column: "AutoPostTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Projects_AutoPostTemplateId",
                table: "Customers",
                column: "AutoPostTemplateId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
