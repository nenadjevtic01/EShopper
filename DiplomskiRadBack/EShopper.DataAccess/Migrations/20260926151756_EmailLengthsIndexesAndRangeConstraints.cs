using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopper.DataAccess.Migrations
{
    public partial class EmailLengthsIndexesAndRangeConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "BillingEmail",
                table: "Receipts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NewsletterSubscribers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PromotionGroups_DiscountPercentage",
                table: "PromotionGroups",
                sql: "[DiscountPercentage] >= 1 AND [DiscountPercentage] <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ProductRatings_Rating",
                table: "ProductRatings",
                sql: "[Rating] >= 1 AND [Rating] <= 5");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_Path",
                table: "NavigationItems",
                column: "Path");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Coupons_DiscountPercentage",
                table: "Coupons",
                sql: "[DiscountPercentage] >= 1 AND [DiscountPercentage] <= 100");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PromotionGroups_DiscountPercentage",
                table: "PromotionGroups");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ProductRatings_Rating",
                table: "ProductRatings");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_NavigationItems_Path",
                table: "NavigationItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Coupons_DiscountPercentage",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BillingEmail",
                table: "Receipts",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NewsletterSubscribers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
