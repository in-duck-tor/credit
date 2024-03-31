using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V10__AddedCreditScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditScore",
                schema: "credit",
                columns: table => new
                {
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditScore", x => x.ClientId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditScore",
                schema: "credit");
        }
    }
}
