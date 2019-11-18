using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroBudget.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ZeroBudget");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetCategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ParentBudgetCategoryId = table.Column<int>(nullable: true),
                    IsTaxDeductible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.BudgetCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPeriodTypes",
                schema: "ZeroBudget",
                columns: table => new
                {
                    BudgetPeriodTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPeriodTypes", x => x.BudgetPeriodTypeId);
                });

            migrationBuilder.CreateTable(
                name: "FrequencyTypes",
                schema: "ZeroBudget",
                columns: table => new
                {
                    FrequencyTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyTypes", x => x.FrequencyTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    BudgetPeriodTypeId = table.Column<int>(nullable: false)
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
                    FrequencyTypeId = table.Column<int>(nullable: true),
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
                values: new object[,]
                {
                    { 1, false, "Salary", null, null },
                    { 2, false, "Utilities", null, null },
                    { 3, false, "Savings", null, null },
                    { 4, false, "Housing", null, null },
                    { 5, false, "Transportation", null, null },
                    { 6, false, "Uncategorized", null, null }
                });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "BudgetPeriodTypes",
                columns: new[] { "BudgetPeriodTypeId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Budget period occurs weekly", "Weekly" },
                    { 2, "Budget period occurs twice a month, every other week ", "Bi-Weekly" },
                    { 3, "Budget period occurs monthly", "Monthly" },
                    { 4, "Budget period occurs twice a month, typically the beginning and middle of the month.", "Semi-Monthly" }
                });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                columns: new[] { "FrequencyTypeId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Occurs the same day each 'n' month(s)", "Monthly" },
                    { 2, "Occurrs the same day each 'n' year(s)", "Annually" },
                    { 3, "Occurs the same day each 'n' week(s)", "Weekly" },
                    { 4, "Occurs the same day each 'n' day(s)", "Daily" },
                    { 5, "Occurs every day monday through Friday", "Monday - Friday" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ActualItems",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetItems",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BudgetCategories",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetPeriods",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "FrequencyTypes",
                schema: "ZeroBudget");

            migrationBuilder.DropTable(
                name: "BudgetPeriodTypes",
                schema: "ZeroBudget");
        }
    }
}
