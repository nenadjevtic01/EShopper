using Microsoft.EntityFrameworkCore;
using EShopper.Domain;

namespace EShopper.DataAccess
{
    public class EShopperDbContext : DbContext
    {
        public EShopperDbContext(DbContextOptions options = null) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<ProductSize>().HasKey(x => new { x.ProductId, x.SizeId });
            modelBuilder.Entity<UserUseCase>().HasKey(x => new { x.UseCaseId, x.UserId });
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StampTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            StampTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void StampTimestamps()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CreatedAt == default)
                    {
                        entry.Entity.CreatedAt = now;
                    }

                    if (entry.Entity.UpdatedAt == default)
                    {
                        entry.Entity.UpdatedAt = now;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Info> Infos { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptsItems { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<NavigationItem> NavigationItems { get; set; }
        public DbSet<HeroSlide> HeroSlides { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<UseCase> UseCases { get; set; }
        public DbSet<UserUseCase> UserUseCases { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionGroup> PromotionGroups { get; set; }
        public DbSet<PromotionGroupProduct> PromotionGroupProducts { get; set; }
    }
}
