﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiClick.Migrations
{
    public partial class suffermore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "u0906946_u0906946");

            migrationBuilder.CreateTable(
                name: "CategoryCl",
                schema: "dbo",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_CategoryCl_CategoryId", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UserCl",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    Points = table.Column<decimal>(nullable: false),
                    NotificationRegistration = table.Column<string>(nullable: true),
                    DeviceType = table.Column<string>(nullable: true),
                    Login = table.Column<string>(maxLength: 250, nullable: true),
                    Password = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Street = table.Column<string>(maxLength: 250, nullable: true),
                    House = table.Column<string>(nullable: true),
                    Padik = table.Column<int>(nullable: true),
                    Etash = table.Column<int>(nullable: true),
                    Kv = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_UserCl_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodCl", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateTable(
                name: "UserRolesCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRolesCl", x => x.UserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "HashtagCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    HashTagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HashTagName = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HashtagCl", x => x.HashTagId);
                    table.ForeignKey(
                        name: "FK_HashtagCl_CategoryCl_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "CategoryCl",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "ImageCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCl", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ImageCl_UserCl_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointRegisterCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    PointRegisterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Points = table.Column<decimal>(nullable: false),
                    TransactionCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRegisterCl", x => x.PointRegisterId);
                    table.ForeignKey(
                        name: "FK_PointRegisterCl_UserCl_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderStatusName = table.Column<string>(nullable: true),
                    MasterRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusCl", x => x.OrderStatusId);
                    table.ForeignKey(
                        name: "FK_OrderStatusCl_MasterRoleId",
                        column: x => x.MasterRoleId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "UserRolesCl",
                        principalColumn: "UserRoleId");
                });

            migrationBuilder.CreateTable(
                name: "BrandCL",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ImgLogoId = table.Column<int>(nullable: false),
                    ImgBannerId = table.Column<int>(nullable: false),
                    BrandName = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    DescriptionMax = table.Column<string>(maxLength: 250, nullable: true),
                    Available = table.Column<bool>(nullable: false),
                    Contact = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Phone = table.Column<string>(maxLength: 250, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    WorkTime = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Rating = table.Column<int>(nullable: true),
                    Rules = table.Column<string>(nullable: true),
                    Hashtag1Id = table.Column<int>(nullable: true),
                    Hashtag2Id = table.Column<int>(nullable: true),
                    Hashtag3Id = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCl_BrandId", x => x.BrandId);
                    table.ForeignKey(
                        name: "FK_BrandCl_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "CategoryCl",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_BrandCl_Hashtag1Id",
                        column: x => x.Hashtag1Id,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "HashtagCl",
                        principalColumn: "HashTagId");
                    table.ForeignKey(
                        name: "FK_BrandCl_Hashtag2Id",
                        column: x => x.Hashtag2Id,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "HashtagCl",
                        principalColumn: "HashTagId");
                    table.ForeignKey(
                        name: "FK_BrandCl_Hashtag3Id",
                        column: x => x.Hashtag3Id,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "HashtagCl",
                        principalColumn: "HashTagId");
                    table.ForeignKey(
                        name: "FK_BrandCl_ImgBannerId",
                        column: x => x.ImgBannerId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "ImageCl",
                        principalColumn: "ImageId");
                    table.ForeignKey(
                        name: "FK_BrandCl_ImgLogoId",
                        column: x => x.ImgLogoId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "ImageCl",
                        principalColumn: "ImageId");
                    table.ForeignKey(
                        name: "FK_BrandCl_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "OrdersCl",
                schema: "dbo",
                columns: table => new
                {
                    OrdersId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<int>(nullable: false),
                    PointsUsed = table.Column<bool>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    BrandOwnerId = table.Column<int>(nullable: true),
                    Commentary = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    House = table.Column<string>(nullable: true),
                    Padik = table.Column<int>(nullable: true),
                    Etash = table.Column<int>(nullable: true),
                    Kv = table.Column<int>(nullable: true),
                    PointRegisterId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_OrdersCl_OrdersId", x => x.OrdersId);
                    table.ForeignKey(
                        name: "FK_OrderCl_BrandOwnerId",
                        column: x => x.BrandOwnerId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_OrderCl_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "CategoryCl",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_OrderCl_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "PaymentMethodCl",
                        principalColumn: "PaymentMethodId");
                    table.ForeignKey(
                        name: "FK_OrderCl_PointRegisterId",
                        column: x => x.PointRegisterId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "PointRegisterCl",
                        principalColumn: "PointRegisterId");
                    table.ForeignKey(
                        name: "FK_OrderCl_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "OrderStatusCl",
                        principalColumn: "OrderStatusId");
                    table.ForeignKey(
                        name: "FK_OrderCl_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BrandMenuCl",
                schema: "dbo",
                columns: table => new
                {
                    BrandMenuId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(nullable: true),
                    ImgId = table.Column<int>(nullable: true),
                    BrandMenuName = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMenuCl_BrandMenuId", x => x.BrandMenuId);
                    table.ForeignKey(
                        name: "FK_BrandMenuCl_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "BrandCL",
                        principalColumn: "BrandId");
                    table.ForeignKey(
                        name: "FK_BrandMenuCl_ImgId",
                        column: x => x.ImgId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "ImageCl",
                        principalColumn: "ImageId");
                });

            migrationBuilder.CreateTable(
                name: "MessageCl",
                schema: "dbo",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    BrandId = table.Column<int>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    Dislikes = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Views = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCl_MessageId", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MessageCl_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "BrandCL",
                        principalColumn: "BrandId");
                    table.ForeignKey(
                        name: "FK_MessageCl_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "WaterRequests",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    WaterRequestId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    BrandId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterRequests", x => x.WaterRequestId);
                    table.ForeignKey(
                        name: "FK_WaterRequests_BrandCL_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "BrandCL",
                        principalColumn: "BrandId");
                    table.ForeignKey(
                        name: "FK_WaterRequests_OrdersCl_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdersCl",
                        principalColumn: "OrdersId");
                });

            migrationBuilder.CreateTable(
                name: "ProductCl",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(nullable: false),
                    BrandMenuId = table.Column<int>(nullable: true),
                    PriceDiscount = table.Column<int>(nullable: true),
                    ImgId = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCl_ProductId", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_ProductCl_BrandMenuId",
                        column: x => x.BrandMenuId,
                        principalSchema: "dbo",
                        principalTable: "BrandMenuCl",
                        principalColumn: "BrandMenuId");
                    table.ForeignKey(
                        name: "FK_ProductCl_ImgId",
                        column: x => x.ImgId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "ImageCl",
                        principalColumn: "ImageId");
                });

            migrationBuilder.CreateTable(
                name: "MessageOpinionCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    MessageOpinionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Opinion = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageOpinionCl", x => x.MessageOpinionId);
                    table.ForeignKey(
                        name: "FK_MessageOpinion_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "dbo",
                        principalTable: "MessageCl",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageOpinion_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCl",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailCl", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdersCl",
                        principalColumn: "OrdersId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "ProductCl",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestDetails",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    RequestDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    SuggestedPrice = table.Column<decimal>(nullable: false),
                    WaterRequestId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDetails", x => x.RequestDetailId);
                    table.ForeignKey(
                        name: "FK_RequestDetails_ProductCl_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "ProductCl",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_RequestDetails_WaterRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "WaterRequests",
                        principalColumn: "WaterRequestId");
                    table.ForeignKey(
                        name: "FK_RequestDetails_WaterRequests_WaterRequestId",
                        column: x => x.WaterRequestId,
                        principalSchema: "u0906946_u0906946",
                        principalTable: "WaterRequests",
                        principalColumn: "WaterRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_CategoryId",
                schema: "dbo",
                table: "BrandCL",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_Hashtag1Id",
                schema: "dbo",
                table: "BrandCL",
                column: "Hashtag1Id");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_Hashtag2Id",
                schema: "dbo",
                table: "BrandCL",
                column: "Hashtag2Id");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_Hashtag3Id",
                schema: "dbo",
                table: "BrandCL",
                column: "Hashtag3Id");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_ImgBannerId",
                schema: "dbo",
                table: "BrandCL",
                column: "ImgBannerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_ImgLogoId",
                schema: "dbo",
                table: "BrandCL",
                column: "ImgLogoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_UserId",
                schema: "dbo",
                table: "BrandCL",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMenuCl_BrandId",
                schema: "dbo",
                table: "BrandMenuCl",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMenuCl_ImgId",
                schema: "dbo",
                table: "BrandMenuCl",
                column: "ImgId",
                unique: true,
                filter: "[ImgId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageCl_BrandId",
                schema: "dbo",
                table: "MessageCl",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageCl_UserId",
                schema: "dbo",
                table: "MessageCl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_BrandOwnerId",
                schema: "dbo",
                table: "OrdersCl",
                column: "BrandOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_CategoryId",
                schema: "dbo",
                table: "OrdersCl",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_PaymentMethodId",
                schema: "dbo",
                table: "OrdersCl",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_PointRegisterId",
                schema: "dbo",
                table: "OrdersCl",
                column: "PointRegisterId",
                unique: true,
                filter: "[PointRegisterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_StatusId",
                schema: "dbo",
                table: "OrdersCl",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCl_UserId",
                schema: "dbo",
                table: "OrdersCl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCl_BrandMenuId",
                schema: "dbo",
                table: "ProductCl",
                column: "BrandMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCl_ImgId",
                schema: "dbo",
                table: "ProductCl",
                column: "ImgId",
                unique: true,
                filter: "[ImgId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "DF_UserCl_Phone_Unique",
                schema: "dbo",
                table: "UserCl",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HashtagCl_CategoryId",
                schema: "u0906946_u0906946",
                table: "HashtagCl",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageCl_UserId",
                schema: "u0906946_u0906946",
                table: "ImageCl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageOpinionCl_MessageId",
                schema: "u0906946_u0906946",
                table: "MessageOpinionCl",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageOpinionCl_UserId",
                schema: "u0906946_u0906946",
                table: "MessageOpinionCl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailCl_OrderId",
                schema: "u0906946_u0906946",
                table: "OrderDetailCl",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailCl_ProductId",
                schema: "u0906946_u0906946",
                table: "OrderDetailCl",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusCl_MasterRoleId",
                schema: "u0906946_u0906946",
                table: "OrderStatusCl",
                column: "MasterRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PointRegisterCl_OwnerId",
                schema: "u0906946_u0906946",
                table: "PointRegisterCl",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_ProductId",
                schema: "u0906946_u0906946",
                table: "RequestDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_RequestId",
                schema: "u0906946_u0906946",
                table: "RequestDetails",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_WaterRequestId",
                schema: "u0906946_u0906946",
                table: "RequestDetails",
                column: "WaterRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterRequests_BrandId",
                schema: "u0906946_u0906946",
                table: "WaterRequests",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterRequests_OrderId",
                schema: "u0906946_u0906946",
                table: "WaterRequests",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageOpinionCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "OrderDetailCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "RequestDetails",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "MessageCl",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductCl",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterRequests",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "BrandMenuCl",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrdersCl",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PaymentMethodCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "PointRegisterCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "OrderStatusCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "HashtagCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "ImageCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "UserRolesCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "CategoryCl",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserCl",
                schema: "dbo");
        }
    }
}
