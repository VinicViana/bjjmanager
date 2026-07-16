import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ChatMessage } from '../models/chat.model';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly http = inject(HttpClient);

  send(messages: ChatMessage[]) {
    return this.http.post<ChatMessage>(`${environment.apiBaseUrl}/chat`, { messages });
  }
}
