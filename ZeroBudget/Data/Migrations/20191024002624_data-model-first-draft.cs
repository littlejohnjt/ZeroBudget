using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroBudget.Data.Migrations
{
    public partial class datamodelfirstdraft : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "ZeroBudget",
                table: "BudgetCategories",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BudgetPeriodTypes",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetPeriodTypeId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPeriodTypes", x => x.BudgetPeriodTypeId);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPeriods",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetPeriodId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    BudgetPeriodTypeId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPeriods", x => x.BudgetPeriodId);
                    table.ForeignKey(
                        name: "FK_BudgetPeriods_BudgetPeriodTypes_BudgetPeriodTypeId",
                        column: x => x.BudgetPeriodTypeId,
                        principalSchema: "ZeroBudget",
                        principalTable: "BudgetPeriodTypes",
                        principalColumn: "BudgetPeriodTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActualItems",
                schema: "ZeroBudget",
                columns: table => new
                {
                    ActualItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    BudgetCategoryId = table.Column<int>(nullable: false),
                    BudgetPeriodId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    TransactionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualItems", x => x.ActualItemId);
                    table.ForeignKey(
                        name: "FK_ActualItems_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalSchema: "ZeroBudget",
                        principalTable: "BudgetCategories",
                        principalColumn: "BudgetCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActualItems_BudgetPeriods_BudgetPeriodId",
                        column: x => x.BudgetPeriodId,
                        principalSchema: "ZeroBudget",
                        principalTable: "BudgetPeriods",
                        principalColumn: "BudgetPeriodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetItems",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    BudgetCategoryId = table.Column<int>(nullable: false),
                    BudgetPeriodId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    IsReoccurring = table.Column<bool>(nullable: false),
                    FrequencyTypeId = table.Column<byte>(nullable: true),
                    FrequencyQuantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetItems", x => x.BudgetItemId);
                    table.ForeignKey(
                        name: "FK_BudgetItems_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalSchema: "ZeroBudget",
                        principalTable: "BudgetCategories",
                        principalColumn: "BudgetCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetItems_BudgetPeriods_BudgetPeriodId",
                        column: x => x.BudgetPeriodId,
                        principalSchema: "ZeroBudget",
                        principalTable: "BudgetPeriods",
                        principalColumn: "BudgetPeriodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetItems_FrequencyTypes_FrequencyTypeId",
                        column: x => x.FrequencyTypeId,
                        principalSchema: "ZeroBudget",
                        principalTable: "FrequencyTypes",
                        principalColumn: "FrequencyTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "BudgetCategories",
                columns: new[] { "BudgetCategoryId", "IsTaxDeductible", "Name", "ParentBudgetCategoryId", "UserId" },
                values: new object[] { 6, null, "Uncategorized", null, null });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "BudgetPeriodTypes",
                columns: new[] { "BudgetPeriodTypeId", "Description", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Budget period occurs weekly", "Weekly" },
                    { (byte)2, "Budget period occurs twice a month, every other week ", "Bi-Weekly" },
                    { (byte)3, "Budget period occurs monthly", "Monthly" },
                    { (byte)4, "Budget period occurs twice a month, typically the beginning and middle of the month.", "Semi-Monthly" }
                });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs the same day each 'n' month(s)", "Monthly" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurrs the same day each 'n' year(s)", "Annually" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs the same day each 'n' week(s)", "Weekly" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs the same day each 'n' day(s)", "Daily" });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                columns: new[] { "FrequencyTypeId", "Description", "Name" },
                values: new object[] { (byte)5, "Occurs every day monday through Friday", "Monday - Friday" });

            migrationBuilder.CreateIndex(
                name: "IX_ActualItems_BudgetCategoryId",
                schema: "ZeroBudget",
                table: "ActualItems",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualItems_BudgetPeriodId",
                schema: "ZeroBudget",
                table: "ActualItems",
                column: "BudgetPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetCategoryId",
                schema: "ZeroBudget",
                table: "BudgetItems",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetPeriodId",
                schema: "ZeroBudget",
                table: "BudgetItems",
                column: "BudgetPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_FrequencyTypeId",
                schema: "ZeroBudget",
                table: "BudgetItems",
                column: "FrequencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPeriods_BudgetPeriodTypeId",
                schema: "ZeroBudget",
                table: "BudgetPeriods",
                column: "BudgetPeriodTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualItems",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetItems",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetPeriods",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetPeriodTypes",
                schema: "ZeroBudget");

            migrationBuilder.DeleteData(
                schema: "ZeroBudget",
                table: "BudgetCategories",
                keyColumn: "BudgetCategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)5);

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "ZeroBudget",
                table: "BudgetCategories");

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurrs the same day each 'n' year(s)", "Annually" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs the same day each 'n' week(s)", "Weekly" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs the same day each 'n' day(s)", "Daily" });

            migrationBuilder.UpdateData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                keyColumn: "FrequencyTypeId",
                keyValue: (byte)4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Occurs every day monday through Friday", "Monday - Friday" });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                columns: new[] { "FrequencyTypeId", "Description", "Name" },
                values: new object[] { (byte)0, "Occurs the same day each 'n' month(s)", "Monthly" });
        }
    }
}
