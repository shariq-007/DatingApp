using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message msg);
    void DeleteMessage(Message msg);
    Task<Message?> GetMessage(string msgId);
    Task<PaginatedResult<MessageDto>> GetMessagesForMember();
    Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId);
    Task<bool> SaveAllAsync();
}
