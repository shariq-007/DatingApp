using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            "Outbox" => query.Where(m => m.SenderId == msgParams.MemberId && m.IsSenderMsgDeleted == false),
            _=> query.Where(m => m.RecipientId == msgParams.MemberId && m.IsRecipientMsgDeleted == false)
        };

        var msgQuery = query.Select(MessageExtensions.ToDtoProjection());

        return await PaginationHelper.CreateAsync(msgQuery, msgParams.PageNumber, msgParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        await context.Messages
            .Where(m => m.RecipientId == currentMemberId 
                && m.SenderId == recipientId && m.MsgReadOn == null)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.MsgReadOn, DateTime.UtcNow));

        return await context.Messages
            .Where(m => (m.RecipientId == currentMemberId && m.IsRecipientMsgDeleted == false && m.SenderId == recipientId) || 
            (m.SenderId == currentMemberId && m.IsSenderMsgDeleted == false && m.RecipientId == recipientId))
            .OrderBy(m => m.MsgSentOn)
            .Select(MessageExtensions.ToDtoProjection())
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
