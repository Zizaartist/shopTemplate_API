using System;
using ApiClick.Models;
using ApiClick.StaticValues;
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

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<ErrorReport> ErrorReport { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<PointRegister> PointRegister { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Image)
                    .HasName("IX_BrandMenus_ImgId");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Image).HasMaxLength(ModelLengths.LENGTH_MIN);
            });

            modelBuilder.Entity<ErrorReport>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_ErrorReports_UserId");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ErrorReports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ErrorReports_Users_UserId");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrdererId)
                    .HasName("IX_Orders_UserId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryPrice).HasColumnType("decimal(18, 2)");

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

                entity.Property(e => e.Commentary).HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.House).HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.OrdererName).HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.Street).HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderInfo)
                    .HasForeignKey<OrderInfo>(d => d.OrderId)
                    .HasConstraintName("FK_OrderInfo_Order");
            });

            modelBuilder.Entity<PointRegister>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IX_PointRegisters_OrderId");

                entity.Property(e => e.Points).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.PointRegisters)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PointRegister_Order");
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

                entity.Property(e => e.Description).HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.Image).HasMaxLength(ModelLengths.LENGTH_MIN);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_CategoryId");
            });

            modelBuilder.Entity<Review>(entity =>
            {
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
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Review)
                    .HasForeignKey<Review>(d => d.OrderId)
                    .HasConstraintName("FK_Messages_OrderId");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_Reviews_UserId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Phone)
                    .HasName("DF_Users_Phone_Unique")
                    .IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeviceType).HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.NotificationRegistration).HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.NotificationsEnabled)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(1)))");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.Points).HasColumnType("decimal(18, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
