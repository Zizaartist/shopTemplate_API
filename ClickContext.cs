using System;
using System.Linq;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<BrandCl> BrandCl { get; set; }
        public virtual DbSet<BrandMenuCl> BrandMenuCl { get; set; }
        public virtual DbSet<MessageCl> MessageCl { get; set; }
        public virtual DbSet<OrdersCl> OrdersCl { get; set; }
        public virtual DbSet<OrderDetailCl> OrderDetailCl { get; set; }
        public virtual DbSet<ProductCl> ProductCl { get; set; }
        public virtual DbSet<UserCl> UserCl { get; set; }
        public virtual DbSet<MessageOpinionCl> MessageOpinionCl { get; set; }
        public virtual DbSet<ImageCl> ImageCl { get; set; }

        public virtual DbSet<CategoryCl> CategoryCl { get; set; }
        public virtual DbSet<OrderStatusCl> OrderStatusCl { get; set; }
        public virtual DbSet<UserRolesCl> UserRolesCls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ClickDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "u0906946_u0906946");

            modelBuilder.Entity<BrandCl>(entity =>
            {
                entity.HasKey(e => e.BrandId)
                    .HasName("PK_BrandCL_BrandId");

                entity.ToTable("BrandCL", "dbo");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.Contact)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Hashtag1).HasMaxLength(250);

                entity.Property(e => e.Hashtag2).HasMaxLength(250);

                entity.Property(e => e.Hashtag3).HasMaxLength(250);

                entity.Property(e => e.Hashtag4).HasMaxLength(250);

                entity.Property(e => e.Hashtag5).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Price).HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UrlImg1).HasMaxLength(250);

                entity.Property(e => e.UrlImg2).HasMaxLength(250);

                entity.Property(e => e.UrlImg3).HasMaxLength(250);

                entity.Property(e => e.UrlImg4).HasMaxLength(250);

                entity.Property(e => e.UrlImg5).HasMaxLength(250);

                entity.Property(e => e.UrlImgBanner).HasMaxLength(250);

                entity.Property(e => e.UrlImgLogo).HasMaxLength(250);

                entity.Property(e => e.WorkTime)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BrandCl)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BrandCL_UserId");
            });

            modelBuilder.Entity<BrandMenuCl>(entity =>
            {
                entity.HasKey(e => e.BrandMenuId)
                    .HasName("PK_BrandMenuCL_BrandMenuId");

                entity.ToTable("BrandMenuCL", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UrlImg1).HasMaxLength(250);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandMenuCl)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_BrandMenuCL_BrandId");
            });

            modelBuilder.Entity<CategoryCl>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("DF_CategoryCL_CategoryId");

                entity.ToTable("CategoryCL", "dbo");

                entity.Property(e => e.CategoryName).HasMaxLength(250);

                entity.Property(e => e.Code).HasMaxLength(50);
            });

            modelBuilder.Entity<MessageCl>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_MessageCL_MessageId");

                entity.ToTable("MessageCL", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MessageCl)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MessageCL_UserId");
            });

            modelBuilder.Entity<OrdersCl>(entity =>
            {
                entity.HasKey(e => e.OrdersId)
                    .HasName("DF_OrdersCL_OrdersId");

                entity.ToTable("OrdersCL", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrdersCl)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_OrdersCL_UserId");

                //entity.Property(e => e.orderDetails).IsRequired();
            });

            modelBuilder.Entity<OrderDetailCl>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);

                entity.HasOne(d => d.order)
                    .WithMany(p => p.orderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_OrderId");

                entity.HasOne(p => p.product)
                    .WithMany()
                    .HasForeignKey(k => k.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductCl>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_ProductCL_ProductId");

                entity.ToTable("ProductCL", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UrlImg1).HasMaxLength(250);

                entity.HasOne(d => d.BrandMenu)
                    .WithMany(p => p.ProductCl)
                    .HasForeignKey(d => d.BrandMenuId)
                    .HasConstraintName("FK_ProductCL_BrandMenuId");
            });

            modelBuilder.Entity<UserCl>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("DF_UserCL_UserId");

                entity.ToTable("UserCL", "dbo");

                entity.HasIndex(e => e.Phone)
                    .HasName("DF_UserCL_Phone_Unique")
                    .IsUnique();
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Pasword).HasMaxLength(250);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Street).HasMaxLength(250);
            });

            modelBuilder.Entity<OrderStatusCl>(entity =>
            {
                entity.HasKey(e => e.OrderStatusId);
            });

            modelBuilder.Entity<UserRolesCl>(entity =>
            {
                entity.HasKey(e => e.UserRolesId);
            });

            modelBuilder.Entity<MessageOpinionCl>(entity => 
            {
                entity.HasKey(k => k.MessageOpinionId);
                //if message is deleted - like gets removed too
                entity.HasOne(m => m.message).WithMany().HasForeignKey(k => k.MessageId).OnDelete(DeleteBehavior.Cascade);
                //it doesn't apply to users though
                entity.HasOne(u => u.user).WithMany().HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ImageCl>(entity => 
            {
                entity.HasKey(k => k.ImageId);
                entity.HasOne(u => u.user).WithMany().HasForeignKey(k => k.UserId).IsRequired();
                entity.Property(p => p.path).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
