import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { ReceiptService } from '../../../Services/receipt.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-orders',
  templateUrl: './admin-orders.component.html',
  styleUrls: ['./admin-orders.component.scss'],
})
export class AdminOrdersComponent implements OnInit {
  headers: any;
  orders: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = false;
  expandedOrderIds: Set<number> = new Set<number>();
  yearFilter: string = 'all';
  availableYears: number[] = [];

  constructor(
    private receiptService: ReceiptService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;

    withMinDuration(
      this.receiptService.search(undefined, this.includeInactive, this.headers),
    ).subscribe({
      next: (data: any) => {
        this.orders = data.data;
        this.dataLoaded = true;
        this.loading = false;

        this.availableYears = Array.from(
          new Set(
            this.orders.map((o: any) => new Date(o.orderedAt).getFullYear()),
          ),
        ).sort((a, b) => b - a);
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.orders = [];
      },
    });
  }

  get filteredOrders(): any[] {
    if (this.yearFilter === 'all') {
      return this.orders;
    }

    return this.orders.filter(
      (o: any) =>
        new Date(o.orderedAt).getFullYear() === Number(this.yearFilter),
    );
  }

  toggleExpand(receiptId: number): void {
    if (this.expandedOrderIds.has(receiptId)) {
      this.expandedOrderIds.delete(receiptId);
    } else {
      this.expandedOrderIds.add(receiptId);
    }
  }

  async deleteOrder(id: number, isActive: boolean): Promise<void> {
    const message = isActive
      ? `Delete order #${id}? It will be hidden from the storefront and this list unless you show deleted orders.`
      : `Restore order #${id}?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete order' : 'Restore order',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.receiptService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Order deleted.' : 'Order restored.',
        );
        this.loadOrders();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not update order.',
        );
      },
    });
  }
}
