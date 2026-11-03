import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { ReceiptService } from '../../Services/receipt.service';
import { ToastService } from '../../Services/toast.service';

@Component({
  selector: 'app-order-confirmation',
  templateUrl: './order-confirmation.component.html',
  styleUrls: ['./order-confirmation.component.scss'],
})
export class OrderConfirmationComponent implements OnInit {
  headers: any;
  order: any;
  dataLoaded: boolean = false;
  errored: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private receiptService: ReceiptService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/home']);
      return;
    }

    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.receiptService.get(id, this.headers).subscribe({
      next: (data: any) => {
        this.order = data;
        this.dataLoaded = true;
      },
      error: (err: any) => {
        if (err.status === 403) {
          this.toastService.error('You do not have access to that order.');
          this.router.navigate(['/home']);
          return;
        }

        this.dataLoaded = true;
        this.errored = true;
        this.toastService.error('Could not load the order.');
      },
    });
  }
}
