import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { NewsletterService } from '../../../Services/newsletter.service';
import { ToastService } from '../../../Services/toast.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-newsletter',
  templateUrl: './admin-newsletter.component.html',
})
export class AdminNewsletterComponent implements OnInit {
  headers: any;
  subscribers: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  keyword: string = '';

  constructor(
    private newsletterService: NewsletterService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.loadSubscribers();
  }

  loadSubscribers(): void {
    this.loading = true;

    withMinDuration(
      this.newsletterService.search(
        this.keyword || undefined,
        undefined,
        this.headers,
      ),
    ).subscribe({
      next: (data: any) => {
        this.subscribers = data.data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load newsletter subscribers.');
      },
    });
  }

  search(): void {
    this.loadSubscribers();
  }
}
