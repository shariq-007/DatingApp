import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { PaginationResult } from '../../types/pagination';
import { Message } from '../../types/message';
import { Paginator } from "../../shared/paginator/paginator";
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-messages',
  imports: [Paginator, RouterLink, DatePipe],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  private msgService = inject(MessageService);
  protected container = 'Inbox';
  protected fetchedContainer = 'Inbox';
  protected pageNumber = 1;
  protected pageSize = 10;
  protected paginatedMsgs = signal<PaginationResult<Message> | null>(null);

  tabs=[
    {label: 'Inbox', value: 'Inbox'},
    {label: 'Outbox', value: 'Outbox'},
  ]

  ngOnInit(): void {
    this.loadMsgs();
  }

  loadMsgs(){
    this.msgService.getMessages(this.container, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.paginatedMsgs.set(response);
        this.fetchedContainer = this.container;
      }
    })
  }

  delMsg(event: Event, id: string){
    event.stopPropagation();

    const confirmed = confirm('Are you sure you want to delete this message?');

    if (!confirmed) {
      return;
    }
    
    this.msgService.deleteMessage(id).subscribe({
      next: () => {
        const current = this.paginatedMsgs();
        if (current?.items){
          this.paginatedMsgs.update(prev => {
            if (!prev) return null;

            const newItems = prev.items.filter(m => m.id != id) || [];

            return {
              items: newItems,
              metadata: prev.metadata
            }
          })
        }
      }
    })
  }

  get isInbox(){
    return this.fetchedContainer === 'Inbox';
  }

  setContainer(container: string){
    this.container = container;
    this.pageNumber = 1;
    this.loadMsgs();
  }

  onPageChange(event: {pageNumber: number, pageSize: number}){
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageNumber;
    this.loadMsgs();
  }
}
