namespace FoodPicker.Migrations.IdentityMigrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FoodPicker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\IdentityMigrations";
            ContextKey = "FoodPicker.Models.ApplicationDbContext";
        }

        protected override void Seed(FoodPicker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //1. Add admin role
            if (!(context.Roles.Any(r => r.Name == "admin")))
            {
                //role does not exist - create it
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roleToInsert = new IdentityRole { Name = "admin" };
                roleManager.Create(roleToInsert);
            }
            //2. Add user role
            if (!(context.Roles.Any(r => r.Name == "user")))
            {
                //role does not exist - create it
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roleToInsert = new IdentityRole { Name = "user" };
                roleManager.Create(roleToInsert);
            }
            //3. Add owner role
            if (!(context.Roles.Any(r => r.Name == "owner")))
            {
                //role does not exist - create it
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roleToInsert = new IdentityRole { Name = "owner" };
                roleManager.Create(roleToInsert);
            }
            // 4. add admin user
            if (!(context.Users.Any(u => u.UserName == "admin@foodpicker.com")))
            {
                //admin user does not exist - create it
                var userStore = new UserStore<Models.ApplicationUser>(context);
                var userManager = new UserManager<Models.ApplicationUser>(userStore);
                var userToInsert = new Models.ApplicationUser
                {
                    UserName = "admin@foodpicker.com",
                    Email = "admin@foodpicker.com",
                    EmailConfirmed = true,
                };
                userManager.Create(userToInsert, "Admin@123456");

                //assign admin user to admin role
                userManager.AddToRole(userToInsert.Id, "admin");

            }

        }
    }
}

//PM> enable-migrations -ContextTypeName FoodPicker.Models.ApplicationDbContext -MigrationsDirectory Migrations\IdentityMigrations
//PM> add-migration -ConfigurationTypeName FoodPicker.Migrations.IdentityMigrations.Configuration "RoleSeed" -IgnoreChanges
//PM> update-database -ConfigurationTypeName FoodPicker.Migrations.IdentityMigrations.Configuration
