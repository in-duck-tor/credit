using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V8__InlinePaymentDistribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingPayoff_Payment_PaymentDistributionId",
                schema: "credit",
                table: "BillingPayoff");

            migrationBuilder.RenameColumn(
                name: "PaymentDistributionId",
                schema: "credit",
                table: "BillingPayoff",
                newName: "PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_BillingPayoff_PaymentDistributionId",
                schema: "credit",
                table: "BillingPayoff",
                newName: "IX_BillingPayoff_PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingPayoff_Payment_PaymentId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PaymentId",
                principalSchema: "credit",
                principalTable: "Payment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingPayoff_Payment_PaymentId",
                schema: "credit",
                table: "BillingPayoff");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                schema: "credit",
                table: "BillingPayoff",
                newName: "PaymentDistributionId");

            migrationBuilder.RenameIndex(
                name: "IX_BillingPayoff_PaymentId",
                schema: "credit",
                table: "BillingPayoff",
                newName: "IX_BillingPayoff_PaymentDistributionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingPayoff_Payment_PaymentDistributionId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PaymentDistributionId",
                principalSchema: "credit",
                principalTable: "Payment",
                principalColumn: "Id");
        }
    }
}
