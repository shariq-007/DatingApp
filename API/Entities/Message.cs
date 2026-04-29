namespace API.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Content { get; set; }
    public DateTime MsgSentOn { get; set; } = DateTime.UtcNow;
    public DateTime? MsgReadOn { get; set; }
    public bool IsSenderMsgDeleted { get; set; }
    public bool IsRecipientMsgDeleted { get; set; }

    //Nav Properties

    public required string SenderId { get; set; }
    public Member Sender { get; set; } = null!;
    public required string RecipientId { get; set; }
    public Member Recipient { get; set; } = null!;
}
