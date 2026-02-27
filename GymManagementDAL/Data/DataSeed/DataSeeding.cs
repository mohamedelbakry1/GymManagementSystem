using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GymManagementDAL.Data.DataSeed
{
    public static class DataSeeding
    {
        public static async Task<bool> SeedData(GymDbContext _dbContext)
        {
            try
            {
                var PendingMigrations = (await _dbContext.Database.GetPendingMigrationsAsync()).Any();
                if (PendingMigrations)
                    await _dbContext.Database.MigrateAsync();

                var HasPlans = await _dbContext.Plans.AnyAsync();
                var HasCategories = await _dbContext.Categories.AnyAsync();

                if (HasPlans && HasCategories) return false;

                if (!HasPlans)
                {
                    var Plans = await LoadDataFromJsonAsync<Plan>("plans.json");
                    if (Plans.Any())
                        await _dbContext.Plans.AddRangeAsync(Plans);
                }

                if (!HasCategories)
                {
                    var Categories = await LoadDataFromJsonAsync<Category>("categories.json");
                    if (Categories.Any())
                        await _dbContext.Categories.AddRangeAsync(Categories);
                }
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed Data Failed {ex}");
                return false;
            }
        }

        private static async Task<List<T>> LoadDataFromJsonAsync<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);
            if (!File.Exists(filePath)) throw new FileNotFoundException();

            var Data = await File.ReadAllTextAsync(filePath);

            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            return  JsonSerializer.Deserialize<List<T>>(Data, Options) ?? new List<T>();
        }
    }
}
