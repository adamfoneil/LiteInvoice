using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class PMCustomerFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "PaymentMethodCustomers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodCustomers_CustomerId",
                table: "PaymentMethodCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodCustomers_PaymentMethodId_CustomerId",
                table: "PaymentMethodCustomers",
                columns: new[] { "PaymentMethodId", "CustomerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethodCustomers_Customers_CustomerId",
                table: "PaymentMethodCustomers",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethodCustomers_PaymentMethods_PaymentMethodId",
                table: "PaymentMethodCustomers",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethodCustomers_Customers_CustomerId",
                table: "PaymentMethodCustomers");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethodCustomers_PaymentMethods_PaymentMethodId",
                table: "PaymentMethodCustomers");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethodCustomers_CustomerId",
                table: "PaymentMethodCustomers");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethodCustomers_PaymentMethodId_CustomerId",
                table: "PaymentMethodCustomers");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "PaymentMethodCustomers");
        }
    }
}
