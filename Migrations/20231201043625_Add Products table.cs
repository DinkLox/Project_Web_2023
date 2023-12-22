using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_2023.Migrations
{
    public partial class AddProductstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "Money", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedById",
                table: "Products",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d", "00ea233a-21ac-48ff-8685-82da12e48c3d", "Khách hàng", "KHÁCH HÀNG" },
                    { "88140411-838a-448b-80ae-b10f3c203af7", "4b4c6316-db62-49f1-a0ee-c1f87b8184be", "Quản trị viên", "QUẢN TRỊ VIÊN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0d146f64-1fd5-43df-a43b-7047e1badf1b", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/img/default-user.webp", "72c411f2-77e6-45f3-a4fd-09fcab084f1f", new DateTime(2023, 11, 30, 16, 10, 36, 25, DateTimeKind.Utc).AddTicks(2830), new DateOnly(2002, 9, 2), "customer@customer.com", true, "Phan Thị Huyền", null, false, null, "CUSTOMER@CUSTOMER.COM", "CUSTOMER", "AQAAAAEAACcQAAAAEA5iNY30UhOt5UgkiiYHVPpMZ0ZJCPri04mVe9yehIBdYIE7+itKsySGJ6vEZNSPSA==", "0123456789", false, "0abdc9bc-3f55-48fd-9af1-ab714aac05a0", false, "customer" },
                    { "93de1dfe-6f40-43f6-aff6-0a3bae7fd181", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/uploads/images/admin.webp", "2ad98c58-3887-4a23-bccc-7a333abe9082", new DateTime(2023, 11, 30, 16, 10, 36, 25, DateTimeKind.Utc).AddTicks(2810), new DateOnly(2002, 7, 2), "admin@admin.com", true, "Nguyễn Ngọc Anh Tuấn", null, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAEAACcQAAAAELzA0cQQ9aa4jGNM6iMArARm/dPNZKRHHBs2s/J3TN34CyrvImSlXtzAMn3I9prTGg==", "0123456789", false, "3acb8c67-77fe-4336-9311-74b946595398", false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d", "0d146f64-1fd5-43df-a43b-7047e1badf1b" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "88140411-838a-448b-80ae-b10f3c203af7", "93de1dfe-6f40-43f6-aff6-0a3bae7fd181" });
        }
    }
}
