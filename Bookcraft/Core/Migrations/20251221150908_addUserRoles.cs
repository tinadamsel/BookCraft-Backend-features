using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO AspNetRoles VALUES (NewId(),'Admin','Admin',NEWID());INSERT INTO AspNetRoles VALUES (NewId(),'User','User',NEWID());");

        }
    }
}
