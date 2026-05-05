using API.DTOs;
using API.Entities;
using API.Extensions;
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

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams msgParams)
    {
        var query = context.Messages
                .OrderByDescending(m => m.MsgSentOn)
                .AsQueryable();

        query = msgParams.Container switch
        {
            "Outbox" => query.Where(m => m.SenderId == msgParams.MemberId),
            _=> query.Where(m => m.RecipientId == msgParams.MemberId)
        };

        var msgQuery = query.Select(MessageExtensions.ToDtoProjection());

        return await PaginationHelper.CreateAsync(msgQuery, msgParams.PageNumber, msgParams.PageSize);
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
