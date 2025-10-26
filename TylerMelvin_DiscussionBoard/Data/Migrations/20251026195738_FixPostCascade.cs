using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TylerMelvin_DiscussionBoard.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPostCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DiscussionThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionThreads_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscussionThreadId = table.Column<int>(type: "int", nullable: false),
                    ParentPostId = table.Column<int>(type: "int", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_DiscussionThreads_DiscussionThreadId",
                        column: x => x.DiscussionThreadId,
                        principalTable: "DiscussionThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ParentPostId",
                        column: x => x.ParentPostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThreads_ApplicationUserId",
                table: "DiscussionThreads",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThreads_CreatedAt",
                table: "DiscussionThreads",
                column: "CreatedAt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ApplicationUserId_CreatedAt",
                table: "Posts",
                columns: new[] { "ApplicationUserId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DiscussionThreadId",
                table: "Posts",
                column: "DiscussionThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ParentPostId",
                table: "Posts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PostId",
                table: "Posts",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "DiscussionThreads");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
