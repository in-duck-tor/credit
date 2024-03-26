using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V6__RemoveLoanBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodBilling_LoanBilling_LoanBillingId",
                schema: "credit",
                table: "PeriodBilling");

            migrationBuilder.DropTable(
                name: "LoanBilling",
                schema: "credit");

            migrationBuilder.RenameColumn(
                name: "BillingItems_LoanBodyPayoff",
                schema: "credit",
                table: "PeriodBilling",
                newName: "ExpenseItems_LoanBodyPayoff");

            migrationBuilder.RenameColumn(
                name: "BillingItems_Interest",
                schema: "credit",
                table: "PeriodBilling",
                newName: "ExpenseItems_Interest");

            migrationBuilder.RenameColumn(
                name: "BillingItems_ChargingForServices",
                schema: "credit",
                table: "PeriodBilling",
                newName: "ExpenseItems_ChargingForServices");

            migrationBuilder.RenameColumn(
                name: "LoanBillingId",
                schema: "credit",
                table: "PeriodBilling",
                newName: "LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_PeriodBilling_LoanBillingId",
                schema: "credit",
                table: "PeriodBilling",
                newName: "IX_PeriodBilling_LoanId");

            migrationBuilder.RenameColumn(
                name: "BillingItems_LoanBodyPayoff",
                schema: "credit",
                table: "BillingPayoff",
                newName: "ExpenseItems_LoanBodyPayoff");

            migrationBuilder.RenameColumn(
                name: "BillingItems_Interest",
                schema: "credit",
                table: "BillingPayoff",
                newName: "ExpenseItems_Interest");

            migrationBuilder.RenameColumn(
                name: "BillingItems_ChargingForServices",
                schema: "credit",
                table: "BillingPayoff",
                newName: "ExpenseItems_ChargingForServices");

            migrationBuilder.AlterColumn<string>(
                name: "LoanAccountNumber",
                schema: "credit",
                table: "Loan",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "LoanBody_Amount",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanDebt_Amount",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Penalty_Amount",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodAccruals_ChargingForServices",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodAccruals_InterestAccrual",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodAccruals_LoanBodyPayoff",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodAccruals_OneTimePayment",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodAccruals_PeriodEndDate",
                schema: "credit",
                table: "Loan",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodAccruals_PeriodStartDate",
                schema: "credit",
                table: "Loan",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodBilling_Loan_LoanId",
                schema: "credit",
                table: "PeriodBilling",
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
                name: "FK_PeriodBilling_Loan_LoanId",
                schema: "credit",
                table: "PeriodBilling");

            migrationBuilder.DropColumn(
                name: "LoanBody_Amount",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "LoanDebt_Amount",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "Penalty_Amount",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_ChargingForServices",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_InterestAccrual",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_LoanBodyPayoff",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_OneTimePayment",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_PeriodEndDate",
                schema: "credit",
                table: "Loan");

            migrationBuilder.DropColumn(
                name: "PeriodAccruals_PeriodStartDate",
                schema: "credit",
                table: "Loan");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_LoanBodyPayoff",
                schema: "credit",
                table: "PeriodBilling",
                newName: "BillingItems_LoanBodyPayoff");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_Interest",
                schema: "credit",
                table: "PeriodBilling",
                newName: "BillingItems_Interest");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_ChargingForServices",
                schema: "credit",
                table: "PeriodBilling",
                newName: "BillingItems_ChargingForServices");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                schema: "credit",
                table: "PeriodBilling",
                newName: "LoanBillingId");

            migrationBuilder.RenameIndex(
                name: "IX_PeriodBilling_LoanId",
                schema: "credit",
                table: "PeriodBilling",
                newName: "IX_PeriodBilling_LoanBillingId");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_LoanBodyPayoff",
                schema: "credit",
                table: "BillingPayoff",
                newName: "BillingItems_LoanBodyPayoff");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_Interest",
                schema: "credit",
                table: "BillingPayoff",
                newName: "BillingItems_Interest");

            migrationBuilder.RenameColumn(
                name: "ExpenseItems_ChargingForServices",
                schema: "credit",
                table: "BillingPayoff",
                newName: "BillingItems_ChargingForServices");

            migrationBuilder.AlterColumn<string>(
                name: "LoanAccountNumber",
                schema: "credit",
                table: "Loan",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LoanBilling",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LoanBody_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanDebt_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Penalty_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodAccruals_ChargingForServices = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_InterestAccrual = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_LoanBodyPayoff = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_OneTimePayment = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_PeriodEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PeriodAccruals_PeriodStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanBilling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanBilling_Loan_Id",
                        column: x => x.Id,
                        principalSchema: "credit",
                        principalTable: "Loan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodBilling_LoanBilling_LoanBillingId",
                schema: "credit",
                table: "PeriodBilling",
                column: "LoanBillingId",
                principalSchema: "credit",
                principalTable: "LoanBilling",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
