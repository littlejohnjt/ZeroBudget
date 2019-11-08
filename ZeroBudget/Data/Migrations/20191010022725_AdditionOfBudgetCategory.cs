using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroBudget.Data.Migrations
{
    public partial class AdditionOfBudgetCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ZeroBudget");

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetCategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ParentBudgetCategoryId = table.Column<int>(nullable: true),
                    IsTaxDeductible = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.BudgetCategoryId);
                });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "BudgetCategories",
                columns: new[] { "BudgetCategoryId", "IsTaxDeductible", "Name", "ParentBudgetCategoryId" },
                values: new object[,]
                {
                    { 1, null, "Salary", null },
                    { 2, null, "Utilities", null },
                    { 3, null, "Savings", null },
                    { 4, null, "Housing", null },
                    { 5, null, "Transportation", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetCategories",
                schema: "ZeroBudget");
        }
    }
}
