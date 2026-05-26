export type Message = {
  id: string
  senderId: string
  senderDisplayName: string
  senderImgUrl: string
  recipientId: string
  recipientDisplayName: string
  recipientImgUrl: string
  content: string
  msgSentOn: string
  msgReadOn?: string
  currentUserSender?: boolean
}