import { Component, effect, ElementRef, inject, OnInit, signal, ViewChild, viewChild } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { MemberService } from '../../../core/services/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css',
})
export class MemberMessages implements OnInit {
  @ViewChild('msgEndRef') msgEndRef!: ElementRef
  private msgService = inject(MessageService);
  private memberService = inject(MemberService);
  protected msgs = signal<Message[]>([]);
  protected msgContent = '';

  constructor(){
    effect(() => {
      const currentsMsgs = this.msgs();
      if (currentsMsgs.length > 0){
        this.scrollToBottom();
      }
    })
  }

  ngOnInit(): void {
    this.loadMsgs();
  }

  loadMsgs(){
    const memberId = this.memberService.member()?.id;

    if (memberId){
      this.msgService.getMessageThread(memberId).subscribe({
        next: msgs => this.msgs.set(msgs.map(msg => ({
          ...msg,
          currentUserSender: msg.senderId !== memberId
        })))
      })
    }
  }

  sendMsg(){
    const recipientId = this.memberService.member()?.id;
    if (!recipientId) return;

    this.msgService.sendMessage(recipientId, this.msgContent).subscribe({
      next: msg => {
        this.msgs.update(msgs => {
          msg.currentUserSender = true
          return [...msgs, msg]
        });
        this.msgContent='';
      }
    })
  }

  scrollToBottom(){
    setTimeout(() => {
      if (this.msgEndRef){
      this.msgEndRef.nativeElement.scrollIntoView({behavior: 'smooth'})
      }
    })
  }
}