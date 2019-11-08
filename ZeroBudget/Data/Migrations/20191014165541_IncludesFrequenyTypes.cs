using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroBudget.Data.Migrations
{
    public partial class IncludesFrequenyTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrequencyTypes",
                schema: "ZeroBudget",
                columns: table => new
                {
                    FrequencyTypeId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyTypes", x => x.FrequencyTypeId);
                });

            migrationBuilder.InsertData(
                schema: "ZeroBudget",
                table: "FrequencyTypes",
                columns: new[] { "FrequencyTypeId", "Description", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Occurs the same day each 'n' month(s)", "Monthly" },
                    { (byte)1, "Occurrs the same day each 'n' year(s)", "Annually" },
                    { (byte)2, "Occurs the same day each 'n' week(s)", "Weekly" },
                    { (byte)3, "Occurs the same day each 'n' day(s)", "Daily" },
                    { (byte)4, "Occurs every day monday through Friday", "Monday - Friday" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrequencyTypes",
                schema: "ZeroBudget");
        }
    }
}
