using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeyBiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKey_AspNetUsers_UserId",
                table: "ApiKey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey");

            migrationBuilder.RenameTable(
                name: "ApiKey",
                newName: "ApiKeys");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ApiKeys",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKey_UserId",
                table: "ApiKeys",
                newName: "IX_ApiKeys_BusinessId");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ApiKeys",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Businesses_UserId",
                table: "Businesses",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys",
                column: "Key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Businesses_BusinessId",
                table: "ApiKeys");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Businesses_UserId",
                table: "Businesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                newName: "ApiKey");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "ApiKey",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_BusinessId",
                table: "ApiKey",
                newName: "IX_ApiKey_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ApiKey",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKey_AspNetUsers_UserId",
                table: "ApiKey",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
