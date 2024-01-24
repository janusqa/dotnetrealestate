using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalUsers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://dotnetmastery.com/bluevillaimages/villa3.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://dotnetmastery.com/bluevillaimages/villa1.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://dotnetmastery.com/bluevillaimages/villa4.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://dotnetmastery.com/bluevillaimages/villa5.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://dotnetmastery.com/bluevillaimages/villa2.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalUsers");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg");
        }
    }
}
