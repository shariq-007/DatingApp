using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<IReadOnlyList<Member>> GetMemberAsync()
    {
        return await context.Members.ToListAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Members.Where(m => m.Id == memberId)
        .SelectMany(m => m.Photos).ToListAsync();
    }
 
    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
    }
}
