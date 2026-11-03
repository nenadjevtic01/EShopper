import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ContactMessageService } from '../../../Services/contact-message.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-messages',
  templateUrl: './admin-messages.component.html',
  styleUrls: ['./admin-messages.component.scss'],
})
export class AdminMessagesComponent implements OnInit {
  headers: any;
  messages: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = false;
  answeredFilter: 'all' | 'answered' | 'unanswered' = 'all';
  keyword: string = '';
  expandedMessageId: number | null = null;
  answerForm!: FormGroup;
  answering: boolean = false;

  constructor(
    private contactMessageService: ContactMessageService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.answerForm = new FormGroup<any>({
      answer: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
      ]),
    });

    this.loadMessages();
  }

  loadMessages(): void {
    this.loading = true;
    const isAnswered =
      this.answeredFilter === 'all'
        ? undefined
        : this.answeredFilter === 'answered';

    withMinDuration(
      this.contactMessageService.search(
        this.keyword || undefined,
        this.includeInactive,
        isAnswered,
        this.headers,
      ),
    ).subscribe({
      next: (data: any) => {
        this.messages = data.data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load messages.');
      },
    });
  }

  search(): void {
    this.loadMessages();
  }

  toggleExpand(id: number): void {
    this.expandedMessageId = this.expandedMessageId === id ? null : id;
    this.answerForm.reset();
  }

  submitAnswer(id: number): void {
    if (this.answerForm.invalid) {
      this.answerForm.markAllAsTouched();
      return;
    }

    this.answering = true;

    this.contactMessageService
      .answer(id, this.answerForm.get('answer')?.value, this.headers)
      .subscribe({
        next: () => {
          this.answering = false;
          this.toastService.success('Answer sent.');
          this.loadMessages();
        },
        error: (err: any) => {
          this.answering = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not send answer.',
          );
        },
      });
  }

  async deleteMessage(id: number, isActive: boolean): Promise<void> {
    const message = isActive
      ? `Delete this message? It will be hidden from this list unless you show deleted messages.`
      : `Restore this message?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete message' : 'Restore message',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.contactMessageService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Message deleted.' : 'Message restored.',
        );
        this.loadMessages();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not update message.',
        );
      },
    });
  }
}
