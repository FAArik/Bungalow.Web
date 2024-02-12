using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BungalowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBungalownumber2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    { 201, 2, null },
                    { 202, 2, null },
                    { 203, 2, null },
                    { 301, 3, null },
                    { 302, 3, null }
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
        }
    }
}
