using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyIdInTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyIdId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ApiKeyId",
                table: "Transactions",
                column: "ApiKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ApiKeys_ApiKeyId",
                table: "Transactions",
                column: "ApiKeyId",
                principalTable: "ApiKeys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ApiKeys_ApiKeyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ApiKeyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ApiKeyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ApiKeyIdId",
                table: "Transactions");
        }
    }
}
