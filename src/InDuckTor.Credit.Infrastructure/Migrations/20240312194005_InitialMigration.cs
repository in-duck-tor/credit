using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "credit");

            migrationBuilder.CreateSequence(
                name: "loan_personal_code_seq",
                schema: "credit");

            migrationBuilder.CreateTable(
                name: "Loan",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BorrowedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BorrowingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    PlannedPaymentsNumber = table.Column<int>(type: "integer", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PaymentScheduleType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanProgram",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PaymentScheduleType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProgram", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDistributed = table.Column<bool>(type: "boolean", nullable: false),
                    Penalty = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanBilling",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PeriodAccruals_PeriodStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PeriodAccruals_PeriodEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PeriodAccruals_InterestAccrual = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_LoanBodyPayoff = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_ChargingForServices = table.Column<decimal>(type: "numeric", nullable: true),
                    PeriodAccruals_OneTimePayment = table.Column<decimal>(type: "numeric", nullable: true),
                    LoanBody_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanDebt_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Penalty_Amount = table.Column<decimal>(type: "numeric", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "LoanApplication",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    LoanProgramId = table.Column<long>(type: "bigint", nullable: false),
                    BorrowedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTerm = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ApplicationState = table.Column<int>(type: "integer", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApplication_LoanProgram_LoanProgramId",
                        column: x => x.LoanProgramId,
                        principalSchema: "credit",
                        principalTable: "LoanProgram",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodBilling",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoanBillingId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OneTimePayment = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDebt = table.Column<bool>(type: "boolean", nullable: false),
                    RemainingPayoff_Interest = table.Column<decimal>(type: "numeric", nullable: true),
                    RemainingPayoff_LoanBodyPayoff = table.Column<decimal>(type: "numeric", nullable: true),
                    RemainingPayoff_ChargingForServices = table.Column<decimal>(type: "numeric", nullable: true),
                    BillingItems_ChargingForServices = table.Column<decimal>(type: "numeric", nullable: false),
                    BillingItems_Interest = table.Column<decimal>(type: "numeric", nullable: false),
                    BillingItems_LoanBodyPayoff = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodBilling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodBilling_LoanBilling_LoanBillingId",
                        column: x => x.LoanBillingId,
                        principalSchema: "credit",
                        principalTable: "LoanBilling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingPayoff",
                schema: "credit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PeriodBillingId = table.Column<long>(type: "bigint", nullable: false),
                    BillingItems_Interest = table.Column<decimal>(type: "numeric", nullable: false),
                    BillingItems_LoanBodyPayoff = table.Column<decimal>(type: "numeric", nullable: false),
                    BillingItems_ChargingForServices = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDistributionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingPayoff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingPayoff_Payment_PaymentDistributionId",
                        column: x => x.PaymentDistributionId,
                        principalSchema: "credit",
                        principalTable: "Payment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BillingPayoff_PeriodBilling_PeriodBillingId",
                        column: x => x.PeriodBillingId,
                        principalSchema: "credit",
                        principalTable: "PeriodBilling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillingPayoff_PaymentDistributionId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PaymentDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingPayoff_PeriodBillingId",
                schema: "credit",
                table: "BillingPayoff",
                column: "PeriodBillingId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplication_LoanProgramId",
                schema: "credit",
                table: "LoanApplication",
                column: "LoanProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodBilling_LoanBillingId",
                schema: "credit",
                table: "PeriodBilling",
                column: "LoanBillingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingPayoff",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "LoanApplication",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "PeriodBilling",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "LoanProgram",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "LoanBilling",
                schema: "credit");

            migrationBuilder.DropTable(
                name: "Loan",
                schema: "credit");

            migrationBuilder.DropSequence(
                name: "loan_personal_code_seq",
                schema: "credit");
        }
    }
}
