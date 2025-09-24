using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cheting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChatConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }
    }
}
