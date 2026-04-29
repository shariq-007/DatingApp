namespace API.DTOs;

public class MessageDto
{
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImgUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImgUrl { get; set; }
    public required string Content { get; set; }
    public DateTime MsgSentOn { get; set; } 
    public DateTime? MsgReadOn { get; set; }
}
