using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V3__AddedClientAndScheduleInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "credit",
                table: "Payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LoanId",
                schema: "credit",
                table: "Payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanApplication",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "credit",
                table: "Loan",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PeriodDay",
                schema: "credit",
                table: "Loan",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeriodInterval",
                schema: "credit",
                table: "Loan",
                type: "interval",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_LoanId",
                schema: "credit",
                table: "Payment",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Loan_LoanId",
                schema: "credit",
                table: "Payment",
                column: "LoanId",
                principalSchema: "credit",
                principalTable: "Loan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Loan_LoanId",
                schema: "credit",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_LoanId",
                schema: "credit",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "credit",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "LoanId",
                schema: "credit",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanApplication");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodDay",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodInterval",
                schema: "credit",
                table: "Loan");
        }
    }
}
