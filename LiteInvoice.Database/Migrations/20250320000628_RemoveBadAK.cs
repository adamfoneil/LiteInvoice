using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBadAK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Businesses_UserId",
                table: "Businesses");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Businesses_UserId",
                table: "Businesses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
