using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V7__RenameLoanColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoanDebt_Amount",
                schema: "credit",
                table: "Loan",
                newName: "Debt_Amount");

            migrationBuilder.RenameColumn(
                name: "LoanBody_Amount",
                schema: "credit",
                table: "Loan",
                newName: "Body_Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Debt_Amount",
                schema: "credit",
                table: "Loan",
                newName: "LoanDebt_Amount");

            migrationBuilder.RenameColumn(
                name: "Body_Amount",
                schema: "credit",
                table: "Loan",
                newName: "LoanBody_Amount");
        }
    }
}
