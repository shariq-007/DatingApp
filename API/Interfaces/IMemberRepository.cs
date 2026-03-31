using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    void update(Member member);

    Task<bool> SaveAllAsync();

    Task<PaginatedResult<Member>> GetMemberAsync(MemberParams memberParams);

    Task<Member?> GetMemberByIdAsync(string id);

    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);

    Task<Member?> GetMemberForUpdate(string id);

}
