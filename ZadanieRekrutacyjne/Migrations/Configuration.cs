namespace ZadanieRekrutacyjne.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ZadanieRekrutacyjne.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ZadanieRekrutacyjne.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string rola = "Admin";
            IdentityResult roleResult;

            if (!RoleManager.RoleExists(rola))
            {
                roleResult = RoleManager.Create(new IdentityRole(rola));
            }

        }
    }
}
