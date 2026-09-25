using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairForm.Migrations
{
    /// <inheritdoc />
    public partial class addOpenCloseCommisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptingOrders",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptingOrders",
                table: "Users");
        }
    }
}
