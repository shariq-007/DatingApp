using System;
using API.Entities;

namespace API.Interfaces;

public interface IMemberRepository
{
    void update(Member member);

    Task<bool> SaveAllAsync();

    Task<IReadOnlyList<Member>> GetMemberAsync();

    Task<Member?> GetMemberByIdAsync(string id);

    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);

}
