using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V9__AddBodyAfterPayoffAndRemoveOneTimePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodAccruals_OneTimePayment",
                schema: "credit",
                table: "Loan");

            migrationBuilder.RenameColumn(
                name: "Body_Amount",
                schema: "credit",
                table: "Loan",
                newName: "CurrentBody_Amount");

            migrationBuilder.AddColumn<decimal>(
                name: "BodyAfterPayoffs_Amount",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyAfterPayoffs_Amount",
                schema: "credit",
                table: "Loan");

            migrationBuilder.RenameColumn(
                name: "CurrentBody_Amount",
                schema: "credit",
                table: "Loan",
                newName: "Body_Amount");

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodAccruals_OneTimePayment",
                schema: "credit",
                table: "Loan",
                type: "numeric",
                nullable: true);
        }
    }
}
