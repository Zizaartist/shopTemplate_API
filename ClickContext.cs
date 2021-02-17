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

        //Модели-регистры
        public virtual DbSet<MessageOpinion> MessageOpinions { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<PointRegister> PointRegisters { get; set; }

        //Enum модели
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Hashtag> Hashtags { get; set; }
        public virtual DbSet<Banknote> Banknotes { get; set; }
        
        //Списки-посредники
        public virtual DbSet<HashtagsListElement> HashtagsListElements { get; set; }
        public virtual DbSet<PaymentMethodsListElement> PaymentMethodsListElements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //2nd "Data Source=tcp:apiclickdbserver.database.windows.net,1433;Initial Catalog=ApiClick_db;User Id=azureuser@apiclickdbserver;Password=!CJGBBVF!3662!"
            //1st Server=(localdb)\\MSSQLLocalDB;Database=ClickDB;Trusted_Connection=True;MultipleActiveResultSets=true");
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

                entity.Property(e => e.ImgLogoId)
                    .IsRequired();

                entity.Property(e => e.ImgBannerId)
                    .IsRequired();

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

                entity.Property(e => e.WorkTime)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Brand_UserId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Brand_CategoryId")
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

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("DF_Categories_CategoryId");

                entity.ToTable("Categories", "dbo");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_Messages_MessageId");

                entity.ToTable("Messages", "dbo");

                //Not nullable

                entity.Property(e => e.Likes)
                    .IsRequired();

                entity.Property(e => e.Dislikes)
                    .IsRequired();

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.Views)
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
                    .HasMaxLength(ModelLengths.LENGTH_MAX);

                entity.HasOne(e => e.OrderStatus)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .HasConstraintName("FK_OrderCl_StatusId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.PaymentMethod)
                      .WithMany()
                      .HasForeignKey(k => k.PaymentMethodId)
                      .HasConstraintName("FK_OrderCl_PaymentMethodId")
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

                entity.HasOne(e => e.PointRegister)
                    .WithMany()
                    .HasForeignKey(e => e.PointRegisterId)
                    .HasConstraintName("FK_OrderCl_PointRegisterId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(k => k.CategoryId)
                    .HasConstraintName("FK_OrderCl_CategoryId")
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Banknote)
                    .WithMany()
                    .HasForeignKey(k => k.BanknoteId)
                    .HasConstraintName("FK_OrderCl_BanknoteId")
                    .OnDelete(DeleteBehavior.NoAction);

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
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_OrderDetails_ProductId");
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

                entity.HasOne(e => e.UserRole)
                    .WithMany()
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.NoAction);
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
                
                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .HasConstraintName("FK_AdBannerCl_CategoryId")
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.OrderStatusId);
                
                entity.ToTable("OrderStatuses", "dbo");

                entity.Property(e => e.OrderStatusName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();

                entity.HasOne(e => e.MasterRole)
                      .WithMany()
                      .HasForeignKey(k => k.MasterRoleId)
                      .HasConstraintName("FK_OrderStatuses_MasterRoleId")
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId);
                
                entity.ToTable("UserRoles", "dbo");

                entity.Property(e => e.UserRoleName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();
            });

            modelBuilder.Entity<MessageOpinion>(entity => 
            {
                entity.HasKey(k => k.MessageOpinionId);
                
                entity.ToTable("MessageOpinions", "dbo");

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

            modelBuilder.Entity<Image>(entity => 
            {
                entity.HasKey(k => k.ImageId);
                
                entity.ToTable("Images", "dbo");

                entity.HasOne(u => u.User)
                    .WithMany()
                    .HasForeignKey(k => k.UserId)
                    .IsRequired();

                entity.Property(p => p.Path).IsRequired();
            });

            modelBuilder.Entity<PaymentMethod>(entity => 
            {
                entity.HasKey(k => k.PaymentMethodId);

                entity.ToTable("PaymentMethods", "dbo");

                entity.Property(e => e.PaymentMethodName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();
            });

            modelBuilder.Entity<Hashtag>(entity => 
            {
                entity.HasKey(e => e.HashTagId);

                entity.ToTable("Hashtags", "dbo");

                entity.Property(e => e.HashTagName)
                    .HasMaxLength(ModelLengths.LENGTH_SMALL)
                    .IsRequired();

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
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
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Banknote>(entity => 
            {
                entity.HasKey(e => e.BanknoteId);

                entity.ToTable("Banknotes", "dbo");
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
                
                entity.HasOne(e => e.PaymentMethod)
                    .WithMany(e => e.PaymentMethodsListElements)
                    .HasForeignKey(e => e.PaymentMethodId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
