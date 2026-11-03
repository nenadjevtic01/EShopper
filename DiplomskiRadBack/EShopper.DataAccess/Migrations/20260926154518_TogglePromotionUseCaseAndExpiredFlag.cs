using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopper.DataAccess.Migrations
{
    public partial class TogglePromotionUseCaseAndExpiredFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE UseCases SET Description = 'Delete promotion and its scheduled prices.' WHERE Name = 'Delete promotion'");

            migrationBuilder.Sql(@"UPDATE Promotions SET IsActive = 0 WHERE IsActive = 1 AND ValidUntil < CAST(SYSUTCDATETIME() AS date)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
