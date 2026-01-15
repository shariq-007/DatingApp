using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(memberData);

        if (members==null)
        {
            Console.WriteLine("No members in Seed data");
            return;
        }

        


        foreach (var member in members)
        {
            using var hmac = new HMACSHA512();
            
            var User = new AppUser
            {
                Id=member.Id,
                Email=member.Email,
                DisplayName=member.DisplayName,
                ImgURL=member.ImgURL,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt=hmac.Key,
                Member = new Member
                {
                    Id=member.Id,
                    DisplayName=member.DisplayName,
                    Description=member.Description,
                    DOB=member.DOB,
                    ImgURL=member.ImgURL,
                    Gender=member.Gender,
                    City=member.City,
                    Country=member.Country,
                    LastActive=member.LastActive,
                    Created=member.Created
                }
            };
            
            User.Member.Photos.Add(new Photo
            {
                Url=member.ImgURL!,
                MemberId=member.Id

            });

            context.Users.Add(User);
        }
        await context.SaveChangesAsync();

    }

}
