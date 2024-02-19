using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BungalowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class amenity_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecialDetails",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 1,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 2,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 3,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 4,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 5,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 6,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 7,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 8,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 9,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 10,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "Id",
                keyValue: 11,
                column: "SpecialDetails",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_BungalowId",
                table: "Amenities",
                column: "BungalowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Bungalows_BungalowId",
                table: "Amenities",
                column: "BungalowId",
                principalTable: "Bungalows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Bungalows_BungalowId",
                table: "Amenities");

            migrationBuilder.DropIndex(
                name: "IX_Amenities_BungalowId",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "SpecialDetails",
                table: "Amenities");
        }
    }
}
