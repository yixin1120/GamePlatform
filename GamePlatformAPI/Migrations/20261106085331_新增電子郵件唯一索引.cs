using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamePlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class 新增電子郵件唯一索引 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_玩家資料_電子郵件",
                table: "玩家資料",
                column: "電子郵件",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_玩家資料_電子郵件",
                table: "玩家資料");
        }
    }
}
