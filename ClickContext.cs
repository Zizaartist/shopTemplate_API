using System;
using System.Linq;
using ApiClick.Models;
using ApiClick.Models.ArrayModels;
using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using ApiClick.StaticValues;
using Click.Models;
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

        //Обычные модели
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<BrandMenu> BrandMenus { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WaterRequest> WaterRequests { get; set; }
        public virtual DbSet<RequestDetail> RequestDetails { get; set; }
        public virtual DbSet<AdBanner> AdBanners { get; set; }
        public virtual DbSet<Report> Reports { get; set; }

        //Модели-регистры
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<PointRegister> PointRegisters { get; set; }

        //Enum модели
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Hashtag> Hashtags { get; set; }
        
        //Списки-посредники
        public virtual DbSet<HashtagsListElement> HashtagsListElements { get; set; }
        public virtual DbSet<PaymentMethodsListElement> PaymentMethodsListElements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "dbo");

            //modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.BrandId)
                    .HasName("PK_Brand_BrandId");

                //Not nullable

                entity.ToTable("Brands", "dbo");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsUnicode();

                entity.Property(e => e.Description)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.DescriptionMax)
                    .HasMaxLength(ModelLengths.LENGTH_MAX);

                entity.Property(e => e.Rules)
                    .HasMaxLength(ModelLengths.LENGTH_MAX);

                entity.Property(e => e.Available)
                    .HasDefaultValue(true);

                //Nullable

                entity.Property(e => e.Contact)
                    .HasMaxLength(ModelLengths.LENGTH_MAX)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.OpenTime)
                    .IsRequired();

                entity.Property(e => e.CloseTime)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Brand_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.ImgLogo)
                    .WithOne()
                    .HasForeignKey<Brand>(e => e.ImgLogoId)
                    .HasConstraintName("FK_Brand_ImgLogoId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.ImgBanner)
                    .WithOne()
                    .HasForeignKey<Brand>(e => e.ImgBannerId)
                    .HasConstraintName("FK_Brand_ImgBannerId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(e => e.HashtagsListElements)
                    .WithOne(e => e.Brand)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(e => e.PaymentMethodsListElements)
                    .WithOne(e => e.Brand)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BrandMenu>(entity =>
            {
                //Not nullable
                entity.HasKey(e => e.BrandMenuId)
                    .HasName("PK_BrandMenus_BrandMenuId");

                entity.ToTable("BrandMenus", "dbo");

                entity.Property(e => e.BrandMenuName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                //Nullable

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandMenus)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_BrandMenus_BrandId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Image)
                    .WithOne()
                    .HasForeignKey<BrandMenu>(d => d.ImgId)
                    .HasConstraintName("FK_BrandMenus_ImgId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_Messages_MessageId");

                entity.ToTable("Messages", "dbo");

                //Not nullable

                entity.Property(e => e.Rating)
                    .IsRequired();

                //Nullable

                entity.Property(e => e.Text)
                    .HasMaxLength(ModelLengths.LENGTH_MAX)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Messages_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Brand)
                    .WithMany()
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Messages_BrandId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Order)
                    .WithOne()
                    .HasForeignKey<Order>(d => d.OrderId)
                    .HasConstraintName("FK_Messages_OrderId")
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("DF_Orders_OrdersId");

                entity.ToTable("Orders", "dbo");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Phone)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.Street)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.House)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.Commentary)
                    .HasMaxLength(ModelLengths.LENGTH_MAX)
                    .IsRequired(false);

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
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.PointRegister)
                    .WithMany()
                    .HasForeignKey(e => e.PointRegisterId)
                    .HasConstraintName("FK_OrderCl_PointRegisterId")
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);
                
                entity.ToTable("OrderDetails", "dbo");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_OrderId");

                entity.HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(k => k.ProductId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_OrderDetails_ProductId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_Products_ProductId");

                entity.ToTable("Products", "dbo");

                entity.Property(e => e.Description)
                    .HasMaxLength(ModelLengths.LENGTH_MAX);

                entity.Property(e => e.Price)
                    .IsRequired();

                entity.Property(e => e.ProductName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<Product>(e => e.ImgId)
                    .HasConstraintName("FK_Products_ImgId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.BrandMenu)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandMenuId)
                    .HasConstraintName("FK_Products_BrandMenuId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("DF_Users_UserId");

                entity.ToTable("Users", "dbo");

                entity.HasIndex(e => e.Phone)
                    .HasName("DF_Users_Phone_Unique")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.Login)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.Password)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Street)
                    .HasMaxLength(ModelLengths.LENGTH_MEDIUM);
                
                entity.Property(e => e.House)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);
                
                entity.Property(e => e.NotificationRegistration)
                    .HasMaxLength(ModelLengths.LENGTH_MAX);
                
                entity.Property(e => e.DeviceType)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL);

                entity.Property(e => e.NotificationsEnabled)
                    .HasDefaultValue(true);
            });
            
            modelBuilder.Entity<AdBanner>(entity =>
            {
                entity.HasKey(e => e.AdBannerId);
                
                entity.ToTable("AdBanners", "dbo");
                
                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<AdBanner>(e => e.ImgId)
                    .HasConstraintName("FK_AdBannerCl_ImgId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.OrderStatusId);
                
                entity.ToTable("OrderStatuses", "dbo");

                entity.Property(e => e.OrderStatusName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();
            });

            modelBuilder.Entity<Image>(entity => 
            {
                entity.HasKey(k => k.ImageId);
                
                entity.ToTable("Images", "dbo");

                entity.HasOne(u => u.User)
                    .WithMany(e => e.UploadedImages)
                    .HasForeignKey(k => k.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(p => p.Path).IsRequired();
            });

            modelBuilder.Entity<Hashtag>(entity => 
            {
                entity.HasKey(e => e.HashTagId);

                entity.ToTable("Hashtags", "dbo");

                entity.Property(e => e.HashTagName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();
            });

            modelBuilder.Entity<PointRegister>(entity =>
            {
                entity.HasKey(e => e.PointRegisterId);

                entity.ToTable("PointRegisters", "dbo");
                
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Receiver)
                    .WithMany()
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<WaterRequest>(entity => 
            {
                entity.HasKey(e => e.WaterRequestId);

                entity.ToTable("WaterRequests", "dbo");

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(k => k.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(k => k.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<RequestDetail>(entity => 
            {
                entity.HasKey(e => e.RequestDetailId);

                entity.ToTable("RequestDetails", "dbo");

                entity.HasOne(e => e.Request)
                    .WithMany()
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.NoAction); //статические продукты не должны пропасть
            });

            modelBuilder.Entity<HashtagsListElement>(entity =>
            {
                entity.HasKey(e => e.HashtagsListElementId);

                entity.ToTable("HashtagsListElements", "dbo");

                entity.HasOne(e => e.Brand)
                    .WithMany(e => e.HashtagsListElements)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Hashtag)
                    .WithMany()
                    .HasForeignKey(e => e.HashtagId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PaymentMethodsListElement>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodListElementId);

                entity.ToTable("PaymentMethodsListElement", "dbo");

                entity.HasOne(e => e.Brand)
                    .WithMany(e => e.PaymentMethodsListElements)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Report>(entity => 
            {
                entity.HasKey(e => e.ReportId);
                entity.HasOne(e => e.Brand)
                    .WithMany()
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ProductOfDay)
                    .WithMany()
                    .HasForeignKey(e => e.ProductOfDayId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);

                entity.Property(e => e.Sum)
                    .IsRequired();

                entity.Property(e => e.OrderCount)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
