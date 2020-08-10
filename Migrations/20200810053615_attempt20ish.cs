using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiClick.Migrations
{
    public partial class attempt20ish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "u0906946_u0906946");

            migrationBuilder.CreateTable(
                name: "CategoryCL",
                schema: "dbo",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(maxLength: 250, nullable: true),
                    Code = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_CategoryCL_CategoryId", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UserCL",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    Pasword = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Street = table.Column<string>(maxLength: 250, nullable: true),
                    House = table.Column<int>(nullable: true),
                    Padik = table.Column<int>(nullable: true),
                    Etash = table.Column<int>(nullable: true),
                    Kv = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_UserCL_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderStatusName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusCl", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "UserRolesCls",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    UserRolesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRolesCls", x => x.UserRolesId);
                });

            migrationBuilder.CreateTable(
                name: "BrandCL",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    DescriptionMax = table.Column<string>(nullable: true),
                    UrlImgLogo = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImgBanner = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg1 = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg2 = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg3 = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg4 = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg5 = table.Column<string>(maxLength: 250, nullable: true),
                    Hashtag1 = table.Column<string>(maxLength: 250, nullable: true),
                    Hashtag2 = table.Column<string>(maxLength: 250, nullable: true),
                    Hashtag3 = table.Column<string>(maxLength: 250, nullable: true),
                    Hashtag4 = table.Column<string>(maxLength: 250, nullable: true),
                    Hashtag5 = table.Column<string>(maxLength: 250, nullable: true),
                    Price = table.Column<string>(maxLength: 250, nullable: true),
                    Contact = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Phone = table.Column<string>(maxLength: 250, nullable: true),
                    WorkTime = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Likes = table.Column<int>(nullable: true),
                    DisLike = table.Column<int>(nullable: true),
                    Rating = table.Column<int>(nullable: true),
                    Views = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCL_BrandId", x => x.BrandId);
                    table.ForeignKey(
                        name: "FK_BrandCL_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCL",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageCL",
                schema: "dbo",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: true),
                    DisLike = table.Column<int>(nullable: true),
                    Rating = table.Column<int>(nullable: true),
                    Views = table.Column<int>(nullable: true),
                    BrandId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCL_MessageId", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MessageCL_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCL",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersCL",
                schema: "dbo",
                columns: table => new
                {
                    OrdersId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF_OrdersCL_OrdersId", x => x.OrdersId);
                    table.ForeignKey(
                        name: "FK_OrdersCL_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCL",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandMenuCL",
                schema: "dbo",
                columns: table => new
                {
                    BrandMenuId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    UrlImg1 = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    BrandId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMenuCL_BrandMenuId", x => x.BrandMenuId);
                    table.ForeignKey(
                        name: "FK_BrandMenuCL_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "BrandCL",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
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
                    opinion = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageOpinionCl", x => x.MessageOpinionId);
                    table.ForeignKey(
                        name: "FK_MessageOpinionCl_MessageCL_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "dbo",
                        principalTable: "MessageCL",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageOpinionCl_UserCL_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserCL",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ProductCL",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    UrlImg1 = table.Column<string>(maxLength: 250, nullable: true),
                    Price = table.Column<int>(nullable: false),
                    PriceDiscount = table.Column<int>(nullable: true),
                    BrandMenuIdDiscount = table.Column<int>(nullable: true),
                    BrandMenuIdNabori = table.Column<int>(nullable: true),
                    BrandMenuIdRezerv1 = table.Column<int>(nullable: true),
                    BrandMenuIdRezerv2 = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    BrandMenuId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCL_ProductId", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_ProductCL_BrandMenuId",
                        column: x => x.BrandMenuId,
                        principalSchema: "dbo",
                        principalTable: "BrandMenuCL",
                        principalColumn: "BrandMenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailCl",
                schema: "u0906946_u0906946",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    price = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailCl", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "OrdersCL",
                        principalColumn: "OrdersId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetailCl_ProductCL_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "ProductCL",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandCL_UserId",
                schema: "dbo",
                table: "BrandCL",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMenuCL_BrandId",
                schema: "dbo",
                table: "BrandMenuCL",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageCL_UserId",
                schema: "dbo",
                table: "MessageCL",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersCL_UserId",
                schema: "dbo",
                table: "OrdersCL",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCL_BrandMenuId",
                schema: "dbo",
                table: "ProductCL",
                column: "BrandMenuId");

            migrationBuilder.CreateIndex(
                name: "DF_UserCL_Phone_Unique",
                schema: "dbo",
                table: "UserCL",
                column: "Phone",
                unique: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MessageOpinionCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "OrderDetailCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "OrderStatusCl",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "UserRolesCls",
                schema: "u0906946_u0906946");

            migrationBuilder.DropTable(
                name: "MessageCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrdersCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandMenuCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandCL",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserCL",
                schema: "dbo");
        }
    }
}
