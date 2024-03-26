using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V4__ScheduleFieldsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodDay",
                schema: "credit",
                table: "Loan");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanProgram",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodInterval",
                schema: "credit",
                table: "LoanProgram");

            migrationBuilder.AddColumn<int>(
                name: "PeriodDay",
                schema: "credit",
                table: "Loan",
                type: "integer",
                nullable: true);
        }
    }
}
