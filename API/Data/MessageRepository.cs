using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddMessage(Message msg)
    {
        context.Messages.Add(msg);
    }

    public void DeleteMessage(Message msg)
    {
        context.Messages.Remove(msg);
    }

    public async Task<Message?> GetMessage(string msgId)
    {
        return await context.Messages.FindAsync(msgId);
    }

    public Task<PaginatedResult<MessageDto>> GetMessagesForMember()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
