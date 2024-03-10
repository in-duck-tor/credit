using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V5__AddAccountInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanApplication");

            migrationBuilder.AddColumn<string>(
                name: "ClientAccountNumber",
                schema: "credit",
                table: "LoanApplication",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientAccountNumber",
                schema: "credit",
                table: "Loan",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoanAccountNumber",
                schema: "credit",
                table: "Loan",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientAccountNumber",
                schema: "credit",
                table: "LoanApplication");

            migrationBuilder.DropColumn(
                name: "ClientAccountNumber",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "LoanAccountNumber",
                schema: "credit",
                table: "Loan");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanApplication",
                type: "interval",
                nullable: true);
        }
    }
}
