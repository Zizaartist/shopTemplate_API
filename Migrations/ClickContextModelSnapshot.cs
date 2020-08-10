﻿// <auto-generated />
using System;
using ApiClick;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiClick.Migrations
{
    [DbContext(typeof(ClickContext))]
    partial class ClickContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DisLike")
                        .HasColumnType("int");

                    b.Property<string>("Hashtag1")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Hashtag2")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Hashtag3")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Hashtag4")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Hashtag5")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("Likes")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Price")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg1")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg2")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg3")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg4")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg5")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImgBanner")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImgLogo")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("Views")
                        .HasColumnType("int");

                    b.Property<string>("WorkTime")
                        .HasColumnType("varchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.HasKey("BrandId")
                        .HasName("PK_BrandCL_BrandId");

                    b.HasIndex("UserId");

                    b.ToTable("BrandCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.BrandMenuCl", b =>
                {
                    b.Property<int>("BrandMenuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg1")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("BrandMenuId")
                        .HasName("PK_BrandMenuCL_BrandMenuId");

                    b.HasIndex("BrandId");

                    b.ToTable("BrandMenuCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.CategoryCl", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("CategoryId")
                        .HasName("DF_CategoryCL_CategoryId");

                    b.ToTable("CategoryCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.OrderStatusCl", b =>
                {
                    b.Property<int>("OrderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OrderStatusName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderStatusId");

                    b.ToTable("OrderStatusCl");
                });

            modelBuilder.Entity("ApiClick.Models.EnumModels.UserRolesCl", b =>
                {
                    b.Property<int>("UserRolesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserRoleName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserRolesId");

                    b.ToTable("UserRolesCls");
                });

            modelBuilder.Entity("ApiClick.Models.MessageCl", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BrandId")
                        .HasColumnType("int");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DisLike")
                        .HasColumnType("int");

                    b.Property<int?>("Likes")
                        .HasColumnType("int");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("Views")
                        .HasColumnType("int");

                    b.HasKey("MessageId")
                        .HasName("PK_MessageCL_MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.MessageOpinionCl", b =>
                {
                    b.Property<int>("MessageOpinionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("opinion")
                        .HasColumnType("bit");

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

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("price")
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

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("Status")
                        .HasColumnType("int")
                        .HasMaxLength(250);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrdersId")
                        .HasName("DF_OrdersCL_OrdersId");

                    b.HasIndex("UserId");

                    b.ToTable("OrdersCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.ProductCl", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrandMenuId")
                        .HasColumnType("int");

                    b.Property<int?>("BrandMenuIdDiscount")
                        .HasColumnType("int");

                    b.Property<int?>("BrandMenuIdNabori")
                        .HasColumnType("int");

                    b.Property<int?>("BrandMenuIdRezerv1")
                        .HasColumnType("int");

                    b.Property<int?>("BrandMenuIdRezerv2")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int?>("PriceDiscount")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("UrlImg1")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("ProductId")
                        .HasName("PK_ProductCL_ProductId");

                    b.HasIndex("BrandMenuId");

                    b.ToTable("ProductCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.UserCl", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("Etash")
                        .HasColumnType("int");

                    b.Property<int?>("House")
                        .HasColumnType("int");

                    b.Property<int?>("Kv")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int?>("Padik")
                        .HasColumnType("int");

                    b.Property<string>("Pasword")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("varchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int>("role")
                        .HasColumnType("int");

                    b.HasKey("UserId")
                        .HasName("DF_UserCL_UserId");

                    b.HasIndex("Phone")
                        .IsUnique()
                        .HasName("DF_UserCL_Phone_Unique");

                    b.ToTable("UserCL","dbo");
                });

            modelBuilder.Entity("ApiClick.Models.BrandCl", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("BrandCl")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_BrandCL_UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.BrandMenuCl", b =>
                {
                    b.HasOne("ApiClick.Models.BrandCl", "Brand")
                        .WithMany("BrandMenuCl")
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK_BrandMenuCL_BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.MessageCl", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("MessageCl")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_MessageCL_UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.MessageOpinionCl", b =>
                {
                    b.HasOne("ApiClick.Models.MessageCl", "message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.UserCl", "user")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.OrderDetailCl", b =>
                {
                    b.HasOne("ApiClick.Models.OrdersCl", "order")
                        .WithMany("orderDetails")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderDetails_OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiClick.Models.ProductCl", "product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ApiClick.Models.OrdersCl", b =>
                {
                    b.HasOne("ApiClick.Models.UserCl", "User")
                        .WithMany("OrdersCl")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_OrdersCL_UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApiClick.Models.ProductCl", b =>
                {
                    b.HasOne("ApiClick.Models.BrandMenuCl", "BrandMenu")
                        .WithMany("ProductCl")
                        .HasForeignKey("BrandMenuId")
                        .HasConstraintName("FK_ProductCL_BrandMenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
