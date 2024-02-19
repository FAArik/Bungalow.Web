using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BungalowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class amenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Bungalows",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BungalowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "Id", "BungalowId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, null, "Private Pool" },
                    { 2, 1, null, "Microwave" },
                    { 3, 1, null, "Private Balcony" },
                    { 4, 1, null, "1 king bed and 1 sofa bed" },
                    { 5, 2, null, "Private Plunge Pool" },
                    { 6, 2, null, "Microwave and Mini Refrigerator" },
                    { 7, 2, null, "Private Balcony" },
                    { 8, 2, null, "king bed or 2 double beds" },
                    { 9, 3, null, "Private Pool" },
                    { 10, 3, null, "Jacuzzi" },
                    { 11, 3, null, "Private Balcony" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Bungalows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
