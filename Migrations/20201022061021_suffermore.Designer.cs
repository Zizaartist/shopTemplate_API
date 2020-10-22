﻿// <auto-generated />
using System;
using ApiClick;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiClick.Migrations
{
    [DbContext(typeof(ClickContext))]
    [Migration("20201022061021_suffermore")]
    partial class suffermore
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("u0906946_u0906946")
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApiClick.Models.BrandCl", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(true);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Contact")
                        .HasColumnType("varchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("DescriptionMax")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("Hashtag1Id")
                        .HasColumnType("int");

                    b.Property<int?>("Hashtag2Id")
                        .HasColumnType("int");

                    b.Property<int?>("Hashtag3Id")
                        .HasColumnType("int");

                    b.Property<int>("ImgBannerId")
                        .HasColumnType("int");

                    b.Property<int>("ImgLogoId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Rules")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WorkTime")
                        .HasColumnType("varchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.HasKey("BrandId")
                        .HasName("PK_BrandCl_BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Hashtag1Id");

                    b.HasIndex("Hashtag2Id");

                    b.HasIndex("Hashtag3Id");

                    b.HasIndex("ImgBannerId")
                        .IsUnique();

                    b.HasIndex("ImgLogoId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("BrandCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.BrandMenuCl", b =>
                {
                    b.Property<int>("BrandMenuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BrandId")
                        .HasColumnType("int");

                    b.Property<string>("BrandMenuName")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("ImgId")
                        .HasColumnType("int");

                    b.HasKey("BrandMenuId")
                        .HasName("PK_BrandMenuCl_BrandMenuId");

                    b.HasIndex("BrandId");

                    b.HasIndex("ImgId")
                        .IsUnique()
                        .HasFilter("[ImgId] IS NOT NULL");

                    b.ToTable("BrandMenuCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.CategoryCl", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("CategoryId")
                        .HasName("DF_CategoryCl_CategoryId");

                    b.ToTable("CategoryCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.OrderStatusCl", b =>
                {
                    b.Property<int>("OrderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MasterRoleId")
                        .HasColumnType("int");

                    b.Property<string>("OrderStatusName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderStatusId");

                    b.HasIndex("MasterRoleId");

                    b.ToTable("OrderStatusCl");
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.PaymentMethodCl", b =>
                {
                    b.Property<int>("PaymentMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PaymentMethodName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentMethodId");

                    b.ToTable("PaymentMethodCl");
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.UserRolesCl", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserRoleName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserRoleId");

                    b.ToTable("UserRolesCl");
                });

            modelBuilder.Entity("ApiClick.Models.ImageCl", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("ImageCl");
                });

            modelBuilder.Entity("ApiClick.Models.MessageCl", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("Dislikes")
                        .HasColumnType("int");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("MessageId")
                        .HasName("PK_MessageCl_MessageId");

                    b.HasIndex("BrandId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.MessageOpinionCl", b =>
                {
                    b.Property<int>("MessageOpinionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<bool>("Opinion")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MessageOpinionId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageOpinionCl");
                });

            modelBuilder.Entity("ApiClick.Models.OrderDetailCl", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetailCl");
                });

            modelBuilder.Entity("ApiClick.Models.OrdersCl", b =>
                {
                    b.Property<int>("OrdersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BrandOwnerId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Commentary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("Etash")
                        .HasColumnType("int");

                    b.Property<string>("House")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Kv")
                        .HasColumnType("int");

                    b.Property<int?>("Padik")
                        .HasColumnType("int");

                    b.Property<int>("PaymentMethodId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PointRegisterId")
                        .HasColumnType("int");

                    b.Property<bool>("PointsUsed")
                        .HasColumnType("bit");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrdersId")
                        .HasName("DF_OrdersCl_OrdersId");

                    b.HasIndex("BrandOwnerId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("PaymentMethodId");

                    b.HasIndex("PointRegisterId")
                        .IsUnique()
                        .HasFilter("[PointRegisterId] IS NOT NULL");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserId");

                    b.ToTable("OrdersCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.ProductCl", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BrandMenuId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("ImgId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("PriceDiscount")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("ProductId")
                        .HasName("PK_ProductCl_ProductId");

                    b.HasIndex("BrandMenuId");

                    b.HasIndex("ImgId")
                        .IsUnique()
                        .HasFilter("[ImgId] IS NOT NULL");

                    b.ToTable("ProductCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.RegisterModels.PointRegister", b =>
                {
                    b.Property<int>("PointRegisterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<decimal>("Points")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("TransactionCompleted")
                        .HasColumnType("bit");

                    b.HasKey("PointRegisterId");

                    b.HasIndex("OwnerId");

                    b.ToTable("PointRegisterCl");
                });

            modelBuilder.Entity("ApiClick.Models.RequestDetail", b =>
                {
                    b.Property<int>("RequestDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<decimal>("SuggestedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("WaterRequestId")
                        .HasColumnType("int");

                    b.HasKey("RequestDetailId");

                    b.HasIndex("ProductId");

                    b.HasIndex("RequestId");

                    b.HasIndex("WaterRequestId");

                    b.ToTable("RequestDetails");
                });

            modelBuilder.Entity("ApiClick.Models.UserCl", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("DeviceType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Etash")
                        .HasColumnType("int");

                    b.Property<string>("House")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Kv")
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("NotificationRegistration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Padik")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Points")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("UserId")
                        .HasName("DF_UserCl_UserId");

                    b.HasIndex("Phone")
                        .IsUnique()
                        .HasName("DF_UserCl_Phone_Unique")
                        .HasFilter("[Phone] IS NOT NULL");

                    b.ToTable("UserCl","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.WaterRequest", b =>
                {
                    b.Property<int>("WaterRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.HasKey("WaterRequestId");

                    b.HasIndex("BrandId");

                    b.HasIndex("OrderId");

                    b.ToTable("WaterRequests");
                });

            modelBuilder.Entity("Click.Models.HashTagCl", b =>
                {
                    b.Property<int>("HashTagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("HashTagName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HashTagId");

                    b.HasIndex("CategoryId");

                    b.ToTable("HashtagCl");
                });

            modelBuilder.Entity("ApiClick.Models.BrandCl", b =>
                {
                    b.HasOne("ApiClick.Models.CategoryCl", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_BrandCl_CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Click.Models.HashTagCl", "HashTag1")
                        .WithMany()
                        .HasForeignKey("Hashtag1Id")
                        .HasConstraintName("FK_BrandCl_Hashtag1Id")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Click.Models.HashTagCl", "HashTag2")
                        .WithMany()
                        .HasForeignKey("Hashtag2Id")
                        .HasConstraintName("FK_BrandCl_Hashtag2Id")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Click.Models.HashTagCl", "HashTag3")
                        .WithMany()
                        .HasForeignKey("Hashtag3Id")
                        .HasConstraintName("FK_BrandCl_Hashtag3Id")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ApiClick.Models.ImageCl", "ImgBanner")
                        .WithOne()
                        .HasForeignKey("ApiClick.Models.BrandCl", "ImgBannerId")
                        .HasConstraintName("FK_BrandCl_ImgBannerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.ImageCl", "ImgLogo")
                        .WithOne()
                        .HasForeignKey("ApiClick.Models.BrandCl", "ImgLogoId")
                        .HasConstraintName("FK_BrandCl_ImgLogoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("Brands")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_BrandCl_UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.BrandMenuCl", b =>
                {
                    b.HasOne("ApiClick.Models.BrandCl", "Brand")
                        .WithMany("BrandMenus")
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK_BrandMenuCl_BrandId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ApiClick.Models.ImageCl", "Image")
                        .WithOne()
                        .HasForeignKey("ApiClick.Models.BrandMenuCl", "ImgId")
                        .HasConstraintName("FK_BrandMenuCl_ImgId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.OrderStatusCl", b =>
                {
                    b.HasOne("ApiClick.Models.EnumModels.UserRolesCl", "MasterRole")
                        .WithMany()
                        .HasForeignKey("MasterRoleId")
                        .HasConstraintName("FK_OrderStatusCl_MasterRoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.ImageCl", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.MessageCl", b =>
                {
                    b.HasOne("ApiClick.Models.BrandCl", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK_MessageCl_BrandId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_MessageCl_UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.MessageOpinionCl", b =>
                {
                    b.HasOne("ApiClick.Models.MessageCl", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .HasConstraintName("FK_MessageOpinion_MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_MessageOpinion_UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.OrderDetailCl", b =>
                {
                    b.HasOne("ApiClick.Models.OrdersCl", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderDetails_OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.ProductCl", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_OrderDetails_ProductId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ApiClick.Models.OrdersCl", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "BrandOwner")
                        .WithMany()
                        .HasForeignKey("BrandOwnerId")
                        .HasConstraintName("FK_OrderCl_BrandOwnerId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ApiClick.Models.CategoryCl", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_OrderCl_CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.EnumModels.PaymentMethodCl", "PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId")
                        .HasConstraintName("FK_OrderCl_PaymentMethodId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.RegisterModels.PointRegister", "PointRegister")
                        .WithOne("Order")
                        .HasForeignKey("ApiClick.Models.OrdersCl", "PointRegisterId")
                        .HasConstraintName("FK_OrderCl_PointRegisterId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ApiClick.Models.EnumModels.OrderStatusCl", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_OrderCl_StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_OrderCl_UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.ProductCl", b =>
                {
                    b.HasOne("ApiClick.Models.BrandMenuCl", "BrandMenu")
                        .WithMany("Products")
                        .HasForeignKey("BrandMenuId")
                        .HasConstraintName("FK_ProductCl_BrandMenuId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ApiClick.Models.ImageCl", "Image")
                        .WithOne()
                        .HasForeignKey("ApiClick.Models.ProductCl", "ImgId")
                        .HasConstraintName("FK_ProductCl_ImgId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("ApiClick.Models.RegisterModels.PointRegister", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.RequestDetail", b =>
                {
                    b.HasOne("ApiClick.Models.ProductCl", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.WaterRequest", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.WaterRequest", null)
                        .WithMany("Suggestions")
                        .HasForeignKey("WaterRequestId");
                });

            modelBuilder.Entity("ApiClick.Models.WaterRequest", b =>
                {
                    b.HasOne("ApiClick.Models.BrandCl", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.OrdersCl", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Click.Models.HashTagCl", b =>
                {
                    b.HasOne("ApiClick.Models.CategoryCl", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
