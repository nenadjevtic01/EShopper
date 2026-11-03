using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopper.DataAccess.Migrations
{
    public partial class LinkPricesToPromotions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Prices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PromotionId",
                table: "Prices",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Promotions_PromotionId",
                table: "Prices",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"UPDATE Promotions SET ValidFrom = CAST(CAST(ValidFrom AS date) AS datetime2), ValidUntil = CAST(CAST(ValidUntil AS date) AS datetime2)");

            migrationBuilder.Sql(@"UPDATE pr SET pr.PromotionId = p.Id
FROM Prices pr
INNER JOIN PromotionGroupProducts pgp ON pgp.ProductId = pr.ProductId
INNER JOIN PromotionGroups g ON g.Id = pgp.PromotionGroupId
INNER JOIN Promotions p ON p.Id = g.PromotionId
WHERE pr.PromotionId IS NULL
  AND pr.OldPrice = pgp.BasePrice
  AND ABS(pr.NewPrice - ROUND(pgp.BasePrice * (100 - g.DiscountPercentage) / 100.0, 2)) < 0.005");

            migrationBuilder.Sql(@"INSERT INTO Prices (ProductId, NewPrice, OldPrice, ActiveFrom, PromotionId)
SELECT DISTINCT pgp.ProductId, pgp.BasePrice, NULL, DATEADD(SECOND, -1, DATEADD(DAY, 1, p.ValidUntil)), p.Id
FROM PromotionGroupProducts pgp
INNER JOIN PromotionGroups g ON g.Id = pgp.PromotionGroupId
INNER JOIN Promotions p ON p.Id = g.PromotionId
WHERE p.IsActive = 1 AND DATEADD(SECOND, -1, DATEADD(DAY, 1, p.ValidUntil)) > SYSUTCDATETIME()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM Prices WHERE PromotionId IS NOT NULL AND OldPrice IS NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Promotions_PromotionId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_PromotionId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Prices");
        }
    }
}
