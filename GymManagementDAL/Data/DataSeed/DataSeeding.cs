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
        public static bool SeedData(GymDbContext _dbContext)
        {
            try
            {
                var PendingMigrations = _dbContext.Database.GetPendingMigrations().Any();
                if (PendingMigrations)
                    _dbContext.Database.Migrate();

                var HasPlans = _dbContext.Plans.Any();
                var HasCategories = _dbContext.Categories.Any();

                if (HasPlans && HasCategories) return false;

                if (!HasPlans)
                {
                    var Plans = LoadDataFromJson<Plan>("plan.json");
                    if (Plans.Any())
                        _dbContext.Plans.AddRange(Plans);
                }

                if (!HasCategories)
                {
                    var Categories = LoadDataFromJson<Category>("categories.json");
                    if (Categories.Any())
                        _dbContext.Categories.AddRange(Categories);
                }
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed Data Failed {ex}");
                return false;
            }
        }

        private static List<T> LoadDataFromJson<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", fileName);
            if (File.Exists(filePath)) throw new FileNotFoundException();

            var Data = File.ReadAllText(filePath);

            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? new List<T>();
        }
    }
}
