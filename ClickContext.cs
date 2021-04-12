using System;
using ApiClick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick
{
    public partial class ClickContext : DbContext
    {
        public ClickContext()
        {
        }

        public ClickContext(DbContextOptions<ClickContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdBanner> AdBanner { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<BrandDoc> BrandDoc { get; set; }
        public virtual DbSet<BrandHashtag> BrandHashtag { get; set; }
        public virtual DbSet<BrandInfo> BrandInfo { get; set; }
        public virtual DbSet<BrandPaymentMethod> BrandPaymentMethod { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<ErrorReport> ErrorReport { get; set; }
        public virtual DbSet<Executor> Executor { get; set; }
        public virtual DbSet<Hashtag> Hashtag { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<PointRegister> PointRegister { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<ScheduleListElement> ScheduleListElement { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<WaterBrand> WaterBrand { get; set; }
        public virtual DbSet<WaterOrder> WaterOrder { get; set; }
        public virtual DbSet<WaterRequest> WaterRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdBanner>(entity =>
            {
                entity.HasIndex(e => e.Image)
                    .HasName("IX_AdBanners_ImgId");

                entity.Property(e => e.Image).HasMaxLength(10);

                entity.Property(e => e.RemoveDate).HasColumnType("datetime");

                entity.Property(e => e.Text).HasMaxLength(50);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.AdBanners)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_AdBanners_Brand");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.ExecutorId)
                    .HasName("IX_Brands_UserId");

                entity.Property(e => e.Available)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(1)))");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MinimalPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Executor)
                    .WithOne(p => p.Brand)
                    .HasForeignKey<Brand>(d => d.ExecutorId)
                    .HasConstraintName("FK_Brand_Executor");
            });

            modelBuilder.Entity<BrandDoc>(entity =>
            {
                entity.HasKey(e => e.BrandDocsId);

                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_BrandDoc")
                    .IsUnique();

                entity.Property(e => e.Executor)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Inn)
                    .IsRequired()
                    .HasColumnName("INN")
                    .HasMaxLength(30);

                entity.Property(e => e.LegalAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OfficialName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Ogrn)
                    .IsRequired()
                    .HasColumnName("OGRN")
                    .HasMaxLength(30);

                entity.HasOne(d => d.Brand)
                    .WithOne(p => p.BrandDoc)
                    .HasForeignKey<BrandDoc>(d => d.BrandId)
                    .HasConstraintName("FK_BrandDoc_Brand");
            });

            modelBuilder.Entity<BrandHashtag>(entity =>
            {
                entity.HasKey(e => e.BrandHashtagsId)
                    .HasName("PK_HashtagsListElements");

                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_HashtagsListElements_BrandId");

                entity.HasIndex(e => e.HashtagId)
                    .HasName("IX_HashtagsListElements_HashtagId");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandHashtags)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_BrandHashtags_Brand");

                entity.HasOne(d => d.Hashtag)
                    .WithMany(p => p.BrandHashtags)
                    .HasForeignKey(d => d.HashtagId)
                    .HasConstraintName("FK_HashtagsListElements_Hashtags_HashtagId");
            });

            modelBuilder.Entity<BrandInfo>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_BrandInfo")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Banner).HasMaxLength(10);

                entity.Property(e => e.DeliveryTerms).HasMaxLength(250);

                entity.Property(e => e.Contact).HasMaxLength(250);

                entity.Property(e => e.DeliveryTime).HasMaxLength(30);

                entity.Property(e => e.Description).HasMaxLength(30);

                entity.Property(e => e.Logo).HasMaxLength(10);

                entity.HasOne(d => d.Brand)
                    .WithOne(p => p.BrandInfo)
                    .HasForeignKey<BrandInfo>(d => d.BrandId)
                    .HasConstraintName("FK_BrandInfo_Brand");
            });

            modelBuilder.Entity<BrandPaymentMethod>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_PaymentMethodsListElement_BrandId");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandPaymentMethods)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_BrandPaymentMethods_Brand");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_BrandMenus_BrandId");

                entity.HasIndex(e => e.Image)
                    .HasName("IX_BrandMenus_ImgId");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Image).HasMaxLength(10);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Categories_Brand");
            });

            modelBuilder.Entity<ErrorReport>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_ErrorReports_UserId");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ErrorReports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ErrorReports_Users_UserId");
            });

            modelBuilder.Entity<Executor>(entity =>
            {
                entity.HasIndex(e => e.Login)
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_Executor_User")
                    .IsUnique();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Executor)
                    .HasForeignKey<Executor>(d => d.UserId)
                    .HasConstraintName("FK_Executor_User");
            });

            modelBuilder.Entity<Hashtag>(entity =>
            {
                entity.Property(e => e.HashTagName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_Orders_BrandId");

                entity.HasIndex(e => e.OrdererId)
                    .HasName("IX_Orders_UserId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Orders_Brand");

                entity.HasOne(d => d.Orderer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrdererId)
                    .HasConstraintName("FK_OrderCl_UserId");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_OrderDetails_OrderId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("IX_OrderDetails_ProductId");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_OrderId");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_OrderInfo")
                    .IsUnique();

                entity.Property(e => e.Commentary).HasMaxLength(250);

                entity.Property(e => e.House).HasMaxLength(30);

                entity.Property(e => e.OrdererName).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Street).HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderInfo)
                    .HasForeignKey<OrderInfo>(d => d.OrderId)
                    .HasConstraintName("FK_OrderInfo_Order");
            });

            modelBuilder.Entity<PointRegister>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_PointRegisters_OrderId");

                entity.HasIndex(e => e.ReceiverId)
                    .HasName("IX_PointRegisters_ReceiverId");

                entity.HasIndex(e => e.SenderId)
                    .HasName("IX_PointRegisters_SenderId");

                entity.Property(e => e.Points).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.PointRegisters)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PointRegister_Order");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.PointRegisterReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PointRegisters_Users_ReceiverId");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.PointRegisterSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_PointRegisters_Users_SenderId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("IX_Products_BrandMenuId");

                entity.HasIndex(e => e.Image)
                    .HasName("IX_Products_ImgId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Image).HasMaxLength(10);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_CategoryId");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_Reports_BrandId");

                entity.HasIndex(e => e.ProductOfDayId)
                    .HasName("IX_Reports_ProductOfDayId");

                entity.Property(e => e.ProductOfDaySum).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sum).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Reports_Brand");

                entity.HasOne(d => d.ProductOfDay)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ProductOfDayId)
                    .HasConstraintName("FK_Reports_Products_ProductOfDayId");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_Brand");

                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_Order")
                    .IsUnique();

                entity.HasIndex(e => e.SenderId)
                    .HasName("IX_Sender");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Messages_Brand");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Review)
                    .HasForeignKey<Review>(d => d.OrderId)
                    .HasConstraintName("FK_Messages_OrderId");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_Reviews_UserId");
            });

            modelBuilder.Entity<ScheduleListElement>(entity =>
            {
                entity.HasIndex(e => e.BrandId);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.ScheduleListElements)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_ScheduleListElements_Brand");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Phone)
                    .HasName("DF_Users_Phone_Unique")
                    .IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeviceType).HasMaxLength(30);

                entity.Property(e => e.NotificationRegistration).HasMaxLength(250);

                entity.Property(e => e.NotificationsEnabled)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(1)))");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Points).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserInfo")
                    .IsUnique();

                entity.Property(e => e.House).HasMaxLength(30);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Street).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserInfo)
                    .HasForeignKey<UserInfo>(d => d.UserId)
                    .HasConstraintName("FK_UserInfo_User");
            });

            modelBuilder.Entity<WaterBrand>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_WaterBrand")
                    .IsUnique();

                entity.Property(e => e.Certificate).HasMaxLength(10);

                entity.Property(e => e.ContainerPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.WaterPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Brand)
                    .WithOne(p => p.WaterBrand)
                    .HasForeignKey<WaterBrand>(d => d.BrandId)
                    .HasConstraintName("FK_WaterBrand_Brand");
            });

            modelBuilder.Entity<WaterOrder>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_WaterOrder")
                    .IsUnique();

                entity.Property(e => e.DeliveryDate)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.WaterOrder)
                    .HasForeignKey<WaterOrder>(d => d.OrderId)
                    .HasConstraintName("FK_WaterOrder_Order");
            });

            modelBuilder.Entity<WaterRequest>(entity =>
            {
                entity.HasIndex(e => e.WaterBrandId)
                    .HasName("IX_WaterRequests_BrandId");

                entity.HasIndex(e => e.WaterOrderId)
                    .HasName("IX_WaterRequests_OrderId");

                entity.HasOne(d => d.WaterBrand)
                    .WithMany(p => p.WaterRequests)
                    .HasForeignKey(d => d.WaterBrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WaterRequest_WaterBrand");

                entity.HasOne(d => d.WaterOrder)
                    .WithMany(p => p.WaterRequests)
                    .HasForeignKey(d => d.WaterOrderId)
                    .HasConstraintName("FK_WaterRequest_WaterOrder");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
