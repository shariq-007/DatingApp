using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(IUnitOfWork uow) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var sender = await uow.MemberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
            return BadRequest("Cannot Send This Message");

        var msg = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        uow.MsgRepository.AddMessage(msg);

        if (await uow.Complete()) return msg.ToDto();

        return BadRequest("Failed To Send Message");
        
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery]MessageParams msgParams)
    {
        msgParams.MemberId = User.GetMemberId();
        return await uow.MsgRepository.GetMessagesForMember(msgParams);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        return Ok(await uow.MsgRepository.GetMessageThread(User.GetMemberId(), recipientId));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string Id)
    {
        var memberId = User.GetMemberId();

        var msg = await uow.MsgRepository.GetMessage(Id);

        if (msg == null) return BadRequest("Cannot delete this message");

        if (msg.SenderId != memberId && msg.RecipientId != memberId) return BadRequest("You cannot delete this message");

        if (msg.SenderId == memberId) msg.IsSenderMsgDeleted=true;
        if (msg.RecipientId == memberId) msg.IsRecipientMsgDeleted=true;

        if (msg is {IsSenderMsgDeleted:true, IsRecipientMsgDeleted:true})
        {
            uow.MsgRepository.DeleteMessage(msg);
        }

        if (await uow.Complete()) return Ok();

        return BadRequest("Problem while deleting the message.");
    }
}
