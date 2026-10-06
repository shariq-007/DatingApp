using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR;

[Authorize]
public class MsgHub(IUnitOfWork uow, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request?.Query["userId"].ToString()
            ?? throw new HubException("Other User Not Found");
        var grpName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, grpName);
        await AddToGroup(grpName);

        var msgs = await uow.MsgRepository.GetMessageThread(GetUserId(), otherUser);

        await Clients.Group(grpName).SendAsync("ReceiveMessageThread", msgs);
    }

    public async Task SendMsg(CreateMessageDto createMsgDto)
    {
        var sender = await uow.MemberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMsgDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMsgDto.RecipientId)
            throw new HubException("Cannot Send Message");

        var msg = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMsgDto.Content
        };

        var grpName = GetGroupName(sender.Id, recipient.Id);
        var group = await uow.MsgRepository.GetMsgGrp(grpName);
        var userInGroup = group != null && group.Connections.Any(x => x.UserId == msg.RecipientId);

        if (userInGroup)
        {
            msg.MsgReadOn = DateTime.UtcNow;
        }

        uow.MsgRepository.AddMessage(msg);

        if (await uow.Complete())
        {
            await Clients.Group(grpName).SendAsync("NewMessage", msg.ToDto());
            var connections = await PresenceTracker.GetConnectionForUsers(recipient.Id);

            if (connections != null && connections.Count > 0 && !userInGroup)
            {
                await presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", msg.ToDto());
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await uow.MsgRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<bool> AddToGroup(string grpName)
    {
        var grp = await uow.MsgRepository.GetMsgGrp(grpName);
        var conn = new Connection(Context.ConnectionId, GetUserId());

        if (grp == null)
        {
            grp = new Group(grpName);
            uow.MsgRepository.AddGroup(grp);
        }

        grp.Connections.Add(conn);

        return await uow.Complete();
        
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
