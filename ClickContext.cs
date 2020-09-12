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
                optionsBuilder.UseSqlServer("Data Source=tcp:apiclickdbserver.database.windows.net,1433;Initial Catalog=ApiClick_db;User Id=azureuser@apiclickdbserver;Password=!CJGBBVF!3662!");
                //Server=(localdb)\\MSSQLLocalDB;Database=ClickDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "u0906946_u0906946");

            modelBuilder.Entity<BrandCl>(entity =>
            {
                entity.HasKey(e => e.BrandId)
                    .HasName("PK_BrandCl_BrandId");

                //Not nullable

                entity.ToTable("BrandCL", "dbo");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode();

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DescriptionMax).HasMaxLength(250);

                //Nullable

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Contact)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.WorkTime)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Hashtag1).HasMaxLength(250);

                entity.Property(e => e.Hashtag2).HasMaxLength(250);

                entity.Property(e => e.Hashtag3).HasMaxLength(250);

                entity.Property(e => e.Hashtag4).HasMaxLength(250);

                entity.Property(e => e.Hashtag5).HasMaxLength(250);

                entity.Property(e => e.UrlImg1).HasMaxLength(250);

                entity.Property(e => e.UrlImg2).HasMaxLength(250);

                entity.Property(e => e.UrlImg3).HasMaxLength(250);

                entity.Property(e => e.UrlImg4).HasMaxLength(250);

                entity.Property(e => e.UrlImg5).HasMaxLength(250);

                entity.Property(e => e.ImgLogoId).IsRequired();

                entity.Property(e => e.ImgBannerId).IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BrandCl_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_BrandCl_CategoryId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.ImgLogo)
                    .WithOne()
                    .HasForeignKey<BrandCl>(e => e.ImgLogoId)
                    .HasConstraintName("FK_BrandCl_ImgLogoId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.ImgBanner)
                    .WithOne()
                    .HasForeignKey<BrandCl>(e => e.ImgBannerId)
                    .HasConstraintName("FK_BrandCl_ImgBannerId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BrandMenuCl>(entity =>
            {

                //Not nullable
                entity.HasKey(e => e.BrandMenuId)
                    .HasName("PK_BrandMenuCl_BrandMenuId");

                entity.ToTable("BrandMenuCl", "dbo");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250);

                //Nullable

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandMenus)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_BrandMenuCl_BrandId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Image)
                    .WithOne()
                    .HasForeignKey<BrandMenuCl>(d => d.ImgId)
                    .HasConstraintName("FK_BrandMenuCl_ImgId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CategoryCl>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("DF_CategoryCl_CategoryId");

                entity.ToTable("CategoryCl", "dbo");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(250)
                    .IsRequired();
            });

            modelBuilder.Entity<MessageCl>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_MessageCl_MessageId");

                entity.ToTable("MessageCl", "dbo");

                //Not nullable

                entity.Property(e => e.Likes).IsRequired();

                entity.Property(e => e.Dislikes).IsRequired();

                entity.Property(e => e.Rating).IsRequired();

                entity.Property(e => e.Views).IsRequired();

                //Nullable

                entity.Property(e => e.Text)
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MessageCl_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Brand)
                    .WithMany()
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_MessageCl_BrandId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<OrdersCl>(entity =>
            {
                entity.HasKey(e => e.OrdersId)
                    .HasName("DF_OrdersCl_OrdersId");

                entity.ToTable("OrdersCl", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(e => e.OrderStatus)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .HasConstraintName("FK_OrderCl_StatusId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_OrderCl_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.BrandOwner)
                    .WithMany()
                    .HasForeignKey(e => e.BrandOwnerId)
                    .HasConstraintName("FK_OrderCl_BrandOwnerId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<OrderDetailCl>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_OrderId");

                entity.HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(k => k.ProductId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_OrderDetails_ProductId");
            });

            modelBuilder.Entity<ProductCl>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_ProductCl_ProductId");

                entity.ToTable("ProductCl", "dbo");

                entity.Property(e => e.Description).IsRequired().HasMaxLength(250);

                entity.Property(e => e.Price).IsRequired();

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .HasConstraintName("FK_ProductCl_CategoryId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<ProductCl>(e => e.ImgId)
                    .HasConstraintName("FK_ProductCl_ImgId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.BrandMenu)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandMenuId)
                    .HasConstraintName("FK_ProductCl_BrandMenuId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserCl>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("DF_UserCl_UserId");

                entity.ToTable("UserCl", "dbo");

                entity.HasIndex(e => e.Phone)
                    .HasName("DF_UserCl_Phone_Unique")
                    .IsUnique();

                entity.Property(e => e.Role).IsRequired();

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Login).HasMaxLength(250);

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Street).HasMaxLength(250);
            });

            modelBuilder.Entity<OrderStatusCl>(entity =>
            {
                entity.HasKey(e => e.OrderStatusId);
            });

            modelBuilder.Entity<UserRolesCl>(entity =>
            {
                entity.HasKey(e => e.UserRoleId);
            });

            modelBuilder.Entity<MessageOpinionCl>(entity => 
            {
                entity.HasKey(k => k.MessageOpinionId);

                //if message is deleted - like gets removed too
                entity.HasOne(m => m.Message)
                    .WithMany()
                    .HasForeignKey(k => k.MessageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MessageOpinion_MessageId");
                //it doesn't apply to users though
                entity.HasOne(u => u.User)
                    .WithMany()
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MessageOpinion_UserId");
            });

            modelBuilder.Entity<ImageCl>(entity => 
            {
                entity.HasKey(k => k.ImageId);

                entity.HasOne(u => u.User)
                    .WithMany()
                    .HasForeignKey(k => k.UserId)
                    .IsRequired();

                entity.Property(p => p.Path).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
