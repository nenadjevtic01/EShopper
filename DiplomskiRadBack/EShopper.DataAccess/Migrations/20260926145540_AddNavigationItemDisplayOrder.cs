using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopper.DataAccess.Migrations
{
    public partial class AddNavigationItemDisplayOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "NavigationItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"UPDATE n SET n.DisplayOrder = r.RowNumber
                  FROM NavigationItems n
                  INNER JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber FROM NavigationItems) r ON r.Id = n.Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "NavigationItems");
        }
    }
}
