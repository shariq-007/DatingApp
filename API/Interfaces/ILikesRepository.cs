using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLike(string SourceMemberId, string TargetMemberId);
    Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParam);
    Task<IReadOnlyList<String>> GetCurrentMemberLikeIds(string memberId);
    void DeleteLike(MemberLike like);
    void AddLike(MemberLike like);
    Task<bool> SaveAllChanges();
}
