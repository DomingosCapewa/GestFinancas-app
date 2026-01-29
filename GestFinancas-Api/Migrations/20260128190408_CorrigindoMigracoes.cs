using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestFinancas_Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoMigracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DraftTransaction",
                table: "DraftTransaction");

            migrationBuilder.DropColumn(
                name: "Confidence",
                table: "DraftTransaction");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "DraftTransaction",
                newName: "DraftTransactions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DraftTransactions",
                newName: "Date");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "DraftTransactions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DraftTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DraftTransactions",
                table: "DraftTransactions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DraftTransactions",
                table: "DraftTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "DraftTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DraftTransactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameTable(
                name: "DraftTransactions",
                newName: "DraftTransaction");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "DraftTransaction",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<double>(
                name: "Confidence",
                table: "DraftTransaction",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DraftTransaction",
                table: "DraftTransaction",
                column: "Id");
        }
    }
}
