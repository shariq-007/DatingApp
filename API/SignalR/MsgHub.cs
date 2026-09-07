using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR;

[Authorize]
public class MsgHub(IMessageRepository msgRepository, IMemberRepository memberRepository) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request?.Query["userId"].ToString()
            ?? throw new HubException("Other User Not Found");
        var grpName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, grpName);

        var msgs = await msgRepository.GetMessageThread(GetUserId(), otherUser);

        await Clients.Group(grpName).SendAsync("ReceiveMessageThread");
    }

    public async Task SendMsg(CreateMessageDto createMsgDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMsgDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMsgDto.RecipientId)
            throw new HubException("Cannot Send Message");

        var msg = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMsgDto.Content
        };

        msgRepository.AddMessage(msg);

        if (await msgRepository.SaveAllAsync())
        {
            var grp = GetGroupName(sender.Id, recipient.Id);
            await Clients.Group(grp).SendAsync("NewMessage", msg.ToDto());
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId()
            ?? throw new HubException("Cannot get Member ID");
    }
}
