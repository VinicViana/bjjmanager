import { Component, ElementRef, ViewChild, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { ChatService } from '../../core/chat/chat.service';
import { ChatMessage } from '../../core/models/chat.model';

const MAX_CONTEXT_MESSAGES = 20;

@Component({
  selector: 'app-chat-widget',
  standalone: true,
  imports: [FormsModule, MatButtonModule, MatIconModule],
  templateUrl: './chat-widget.component.html',
  styleUrl: './chat-widget.component.scss'
})
export class ChatWidgetComponent {
  private readonly chatService = inject(ChatService);
  private readonly snackBar = inject(MatSnackBar);

  @ViewChild('messagesContainer') private messagesContainer?: ElementRef<HTMLDivElement>;

  readonly isOpen = signal(false);
  readonly messages = signal<ChatMessage[]>([]);
  readonly sending = signal(false);

  draft = '';

  private pendingRequest?: Subscription;

  constructor() {
    effect(() => {
      this.messages();
      this.sending();
      queueMicrotask(() => this.scrollToBottom());
    });
  }

  toggle(): void {
    this.isOpen.update((open) => !open);
  }

  clear(): void {
    this.pendingRequest?.unsubscribe();
    this.sending.set(false);
    this.messages.set([]);
  }

  send(): void {
    const content = this.draft.trim();

    if (!content || this.sending()) {
      return;
    }

    this.draft = '';
    this.messages.update((current) => [...current, { role: 'user', content }]);
    this.sending.set(true);

    const context = this.messages().slice(-MAX_CONTEXT_MESSAGES);

    this.pendingRequest = this.chatService.send(context).subscribe({
      next: (reply) => {
        this.messages.update((current) => [...current, reply]);
        this.sending.set(false);
      },
      error: () => {
        this.sending.set(false);
        this.snackBar.open('The AI coach is unavailable right now, try again in a moment.', 'Close', {
          duration: 5000
        });
      }
    });
  }

  private scrollToBottom(): void {
    const element = this.messagesContainer?.nativeElement;

    if (element) {
      element.scrollTop = element.scrollHeight;
    }
  }
}
