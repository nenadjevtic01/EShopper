using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopper.DataAccess.Migrations
{
    public partial class BrandAndCategoryChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alt",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Src",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Alt",
                table: "Brands",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Src",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Src",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Alt",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Src",
                table: "Brands");
        }
    }
}
