using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Data.DataSeed;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagementPL.Extensions
{
    public static class WebApplicationExtension
    {
        public static async Task<WebApplication> SeedAndMigrateDataAsync(this WebApplication app)
        {
            using var Scope = app.Services.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var userManager = Scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var PendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).Any();
            if (PendingMigrations)
                await dbContext.Database.MigrateAsync();

            await DataSeeding.SeedData(dbContext);
            await IdentityDataSeeding.SeedData(userManager, roleManager);

            return app;
        }
    }
}
