using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BungalowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBungalownumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bungalows",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bungalows",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "BungalowNumbers",
                columns: table => new
                {
                    Bungalow_Number = table.Column<int>(type: "int", nullable: false),
                    BungalowId = table.Column<int>(type: "int", nullable: false),
                    SpecialDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BungalowNumbers", x => x.Bungalow_Number);
                    table.ForeignKey(
                        name: "FK_BungalowNumbers_Bungalows_BungalowId",
                        column: x => x.BungalowId,
                        principalTable: "Bungalows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BungalowNumbers",
                columns: new[] { "Bungalow_Number", "BungalowId", "SpecialDetails" },
                values: new object[,]
                {
                    { 101, 1, null },
                    { 102, 1, null },
                    { 103, 1, null },
                    { 104, 1, null },
                    { 301, 3, null },
                    { 302, 3, null }
                });

            migrationBuilder.InsertData(
                table: "Bungalows",
                columns: new[] { "Id", "CreatedDate", "Description", "ImageUrl", "Name", "Occupancy", "Price", "Sqft", "UpdatedDate" },
                values: new object[] { 2, null, "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.", "https://placehold.co/600x401", "Premium Pool Bungalow", 4, 300.0, 550, null });

            migrationBuilder.InsertData(
                table: "BungalowNumbers",
                columns: new[] { "Bungalow_Number", "BungalowId", "SpecialDetails" },
                values: new object[,]
                {
                    { 201, 2, null },
                    { 202, 2, null },
                    { 203, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BungalowNumbers_BungalowId",
                table: "BungalowNumbers",
                column: "BungalowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BungalowNumbers");

            migrationBuilder.DeleteData(
                table: "Bungalows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bungalows",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "Bungalows",
                columns: new[] { "Id", "CreatedDate", "Description", "ImageUrl", "Name", "Occupancy", "Price", "Sqft", "UpdatedDate" },
                values: new object[] { -2, null, "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.", "https://placehold.co/600x401", "Premium Pool Bungalow", 4, 300.0, 550, null });
        }
    }
}
