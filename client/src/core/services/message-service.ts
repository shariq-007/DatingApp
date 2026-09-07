import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginationResult } from '../../types/pagination';
import { Message } from '../../types/message';
import { AccountService } from './account-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private baseUrl = environment.apiUrl;
  private hubUrl = environment.hubUrl;
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private hubConnection?: HubConnection;
  msgThread = signal<Message[]>([]);

  createHubConnection(otherUserId: string){
    const currentUser = this.accountService.currentUser();
    if (!currentUser) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'messages?userId=' + otherUserId, {
        accessTokenFactory: () => currentUser.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on('ReceiveMessageThread', (msgs: Message[]) => {
      this.msgThread.set(msgs.map(msg => ({
          ...msg,
          currentUserSender: msg.senderId !== otherUserId
        })))
    });
  }

  stopHubConnection(){
    if (this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(error => console.log(error))
    }
  }

  getMessages(container: string, pageNumber: number, pageSize: number){
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    params = params.append('container', container);

    return this.http.get<PaginationResult<Message>>(this.baseUrl + 'messages', {params});
  }

  getMessageThread(memberId: string){
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + memberId);
  }  

  sendMessage(recipientId: string, content: string){
    return this.http.post<Message>(this.baseUrl + 'messages', {recipientId, content})
  }

  deleteMessage(id: string){
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
