using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_2023.Migrations
{
    public partial class Setuserdateofbirthtonull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b25db9dc-e4dd-417b-b420-cd8d0ded8332", "61438e77-14e2-4314-8bca-5ba8f1af15ee" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "94f53925-fc12-4d8c-b5a3-9e51c596f5d5", "cb234013-a979-40c2-afae-1090be83ae4f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94f53925-fc12-4d8c-b5a3-9e51c596f5d5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b25db9dc-e4dd-417b-b420-cd8d0ded8332");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "61438e77-14e2-4314-8bca-5ba8f1af15ee");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cb234013-a979-40c2-afae-1090be83ae4f");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "94f53925-fc12-4d8c-b5a3-9e51c596f5d5", "48904ed5-7f50-4cfb-b3e4-ca2f7d73d7fd", "Khách hàng", "KHÁCH HÀNG" },
                    { "b25db9dc-e4dd-417b-b420-cd8d0ded8332", "dc3af671-3293-4453-a7e5-088edf265da0", "Quản trị viên", "QUẢN TRỊ VIÊN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "61438e77-14e2-4314-8bca-5ba8f1af15ee", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/uploads/images/admin.webp", "5071c891-354e-4d76-a98e-cbd3c29f719c", new DateTime(2023, 11, 28, 12, 53, 12, 295, DateTimeKind.Utc).AddTicks(8880), new DateOnly(2002, 7, 2), "admin@admin.com", true, "Nguyễn Ngọc Anh Tuấn", null, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAEAACcQAAAAEFE/ni1Cc8Nirb3QVDnFvDBmDYJcRm9mc6l6n/Hg44OpcWPbijD3iU25SFyqPBHXzQ==", "0123456789", false, "3f8391be-223c-4553-a2b7-0400f589c81e", false, "admin" },
                    { "cb234013-a979-40c2-afae-1090be83ae4f", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/img/default-user.webp", "7aebeabe-c7c6-4657-ad18-6a7377e48c7e", new DateTime(2023, 11, 28, 12, 53, 12, 295, DateTimeKind.Utc).AddTicks(8890), new DateOnly(2002, 9, 2), "customer@customer.com", true, "Phan Thị Huyền", null, false, null, "CUSTOMER@CUSTOMER.COM", "CUSTOMER", "AQAAAAEAACcQAAAAELbvUlGA567u1v95ivGfJaZnWAF7ddC05eE13CVqXX6nHMJsks59exNBFlReoTdu2w==", "0123456789", false, "d33be49f-0a32-4a21-80d5-18e77c46b48c", false, "customer" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "b25db9dc-e4dd-417b-b420-cd8d0ded8332", "61438e77-14e2-4314-8bca-5ba8f1af15ee" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "94f53925-fc12-4d8c-b5a3-9e51c596f5d5", "cb234013-a979-40c2-afae-1090be83ae4f" });
        }
    }
}
