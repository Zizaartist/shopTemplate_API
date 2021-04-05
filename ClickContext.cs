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

        public virtual DbSet<AdBanner> AdBanners { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<BrandDoc> BrandDocs { get; set; }
        public virtual DbSet<BrandHashtag> BrandHashtags { get; set; }
        public virtual DbSet<BrandInfo> BrandInfo { get; set; }
        public virtual DbSet<BrandPaymentMethod> BrandPaymentMethods { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ErrorReport> ErrorReports { get; set; }
        public virtual DbSet<Executor> Executors { get; set; }
        public virtual DbSet<Hashtag> Hashtags { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<PointRegister> PointRegisters { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ScheduleListElement> ScheduleListElements { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<WaterBrand> WaterBrands { get; set; }
        public virtual DbSet<WaterOrder> WaterOrders { get; set; }
        public virtual DbSet<WaterRequest> WaterRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=Redesign_Click_DB;Trusted_Connection=True");
            }
        }

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
                    .WithMany(p => p.AdBanner)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_AdBanner_Brand");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.UserId)
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

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Brand)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_UserId");
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
                    .WithMany(p => p.BrandHashtag)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HashtagsListElements_Brands_BrandId");

                entity.HasOne(d => d.Hashtag)
                    .WithMany(p => p.BrandHashtag)
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

                entity.Property(e => e.Conditions).HasMaxLength(250);

                entity.Property(e => e.Contact).HasMaxLength(250);

                entity.Property(e => e.DeliveryTime).HasMaxLength(30);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30);

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
                    .WithMany(p => p.BrandPaymentMethod)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_PaymentMethodsListElement_Brands_BrandId");
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
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandMenus_BrandId");
            });

            modelBuilder.Entity<ErrorReport>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_ErrorReports_UserId");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ErrorReport)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ErrorReports_Users_UserId");
            });

            modelBuilder.Entity<Executor>(entity =>
            {
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_Executor_Brand")
                    .IsUnique();

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

                entity.HasOne(d => d.Brand)
                    .WithOne(p => p.Executor)
                    .HasForeignKey<Executor>(d => d.BrandId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Executor_Brand");

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

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Order_Brand");

                entity.HasOne(d => d.Orderer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.OrdererId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
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
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_OrderId");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_OrderInfo")
                    .IsUnique();

                entity.Property(e => e.Commentary).HasMaxLength(250);

                entity.Property(e => e.House)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.OrdererName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderInfo)
                    .HasForeignKey<OrderInfo>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
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
                    .WithMany(p => p.PointRegister)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PointRegister_Order");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.PointRegisterReceiver)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_PointRegisters_Users_ReceiverId");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.PointRegisterSender)
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
                    .WithMany(p => p.Product)
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
                    .WithMany(p => p.Report)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Report_Brand");

                entity.HasOne(d => d.ProductOfDay)
                    .WithMany(p => p.Report)
                    .HasForeignKey(d => d.ProductOfDayId)
                    .OnDelete(DeleteBehavior.SetNull)
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
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Messages_BrandId");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Review)
                    .HasForeignKey<Review>(d => d.OrderId)
                    .HasConstraintName("FK_Messages_OrderId");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_Messages_UserId");
            });

            modelBuilder.Entity<ScheduleListElement>(entity =>
            {
                entity.HasIndex(e => e.BrandId);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.ScheduleListElement)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_ScheduleListElement_Brands_BrandId");
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
                entity.HasIndex(e => e.BrandId)
                    .HasName("IX_WaterRequests_BrandId");

                entity.HasIndex(e => e.WaterOrderId)
                    .HasName("IX_WaterRequests_OrderId");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.WaterRequest)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_WaterRequests_Brands_BrandId");

                entity.HasOne(d => d.WaterOrder)
                    .WithMany(p => p.WaterRequest)
                    .HasForeignKey(d => d.WaterOrderId)
                    .HasConstraintName("FK_WaterRequest_WaterOrder");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
