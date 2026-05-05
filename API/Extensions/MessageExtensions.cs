using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message msg)
    {
        return new MessageDto
        {
            Id = msg.Id,
            SenderId = msg.SenderId,
            SenderDisplayName = msg.Sender.DisplayName,
            SenderImgUrl = msg.Sender.ImgURL,
            RecipientId = msg.RecipientId,
            RecipientDisplayName = msg.Recipient.DisplayName,
            RecipientImgUrl = msg.Recipient.ImgURL,
            Content = msg.Content,
            MsgReadOn = msg.MsgReadOn,
            MsgSentOn = msg.MsgSentOn
        };
    }

    public static Expression<Func<Message, MessageDto>> ToDtoProjection()
    {
        return msg => new MessageDto
        {
            Id = msg.Id,
            SenderId = msg.SenderId,
            SenderDisplayName = msg.Sender.DisplayName,
            SenderImgUrl = msg.Sender.ImgURL,
            RecipientId = msg.RecipientId,
            RecipientDisplayName = msg.Recipient.DisplayName,
            RecipientImgUrl = msg.Recipient.ImgURL,
            Content = msg.Content,
            MsgReadOn = msg.MsgReadOn,
            MsgSentOn = msg.MsgSentOn
        };
    }
}
