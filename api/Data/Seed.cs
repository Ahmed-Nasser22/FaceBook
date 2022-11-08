using api.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace api.Data
{
    public class Seed
    {

        public static async Task SeedUsers (DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var UserData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            List<AppUser> Users = JsonSerializer.Deserialize < List < AppUser >> (UserData);
            foreach(AppUser user in Users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);
            
            }
            await context.SaveChangesAsync();
        }
    }
}
