using HairForm.Models;
using Microsoft.AspNetCore.Identity;

namespace HairForm.Database
{
    public static class DbInitializer
    {
        public static void Initializer(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if(!database.Users.Any(x=>x.Role == Role.Admin))
            {
                var passwordHash = new PasswordHasher<User>();
                var admin= new User { Id = Guid.NewGuid().ToString(), Name = "Admin", Role = Role.Admin };
                var password = passwordHash.HashPassword(admin, "Admin123");
                admin.Password = password;
                database.Users.Add(admin);
                database.SaveChanges();
            }
        }
    }
}
