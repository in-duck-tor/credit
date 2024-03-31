using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V12__MadePeriodBillingOptionalInBillingPayoff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingPayoff_PeriodBilling_PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff");

            migrationBuilder.AlterColumn<long>(
                name: "PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingPayoff_PeriodBilling_PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PeriodBillingId",
                principalSchema: "credit",
                principalTable: "PeriodBilling",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingPayoff_PeriodBilling_PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff");

            migrationBuilder.AlterColumn<long>(
                name: "PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillingPayoff_PeriodBilling_PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PeriodBillingId",
                principalSchema: "credit",
                principalTable: "PeriodBilling",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
