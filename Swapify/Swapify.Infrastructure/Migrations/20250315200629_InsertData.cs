using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Swapify.Infrastructure.Migrations
{
    public partial class InsertData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[Users] ([UserId], [UserName], [Email], [PasswordHash], [Salt], [FirstName], [LastName], [IsBlocked], [EmailConfirmed])
                VALUES
                ('5490f8e4-7593-4abd-b55b-e9ecbc05e9c6', 'sergiusuciu2002', 'sergiusuciu2002@gmail.com', '$2a$12$xCZfX8xwNTnj9PFLV5OnmO/OIbMvnX4vx2gqGYawuB99C67pnorrW', '$2a$12$xCZfX8xwNTnj9PFLV5OnmO', 'Suciu', 'Sergiu', 0, 1),
                ('d9ddba5c-c5a5-4a9a-813b-986f0741283c', 'sergiu.eduard.suciu', 'sergiu.eduard.suciu@gmail.com', '$2a$12$d.ubZy.Oro8.fZ1nHkQkWeoLONPoqU0EdTqYFQgxmuS47qM1u/3OC', '$2a$12$d.ubZy.Oro8.fZ1nHkQkWe', 'Sergiu', 'Suciu', 0, 1)
            ");
                    migrationBuilder.Sql(@"
                INSERT INTO [dbo].[Roles] ([RoleId], [Name], [Description])
                VALUES
                (1, 'admin', 'An admin role is associated with extensive rights and access within an application or system. Administrators often have full control over the resources and functionalities of the system and can make global changes'),
                (2, 'user', 'A user role refers to a set of permissions and access levels assigned to a regular user within an application or system. Regular users, or those with user roles, typically have limited access to features and resources within the system')
            ");
                    migrationBuilder.Sql(@"
                INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId])
                VALUES
                ('5490f8e4-7593-4abd-b55b-e9ecbc05e9c6', 1),
                ('d9ddba5c-c5a5-4a9a-813b-986f0741283c', 1)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[UserRoles]");
            migrationBuilder.Sql("DELETE FROM [dbo].[Users]");
            migrationBuilder.Sql("DELETE FROM [dbo].[Roles]");
        }
    }
}
